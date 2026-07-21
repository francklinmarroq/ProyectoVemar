Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports Microsoft.Reporting.NETCore
Imports Vemar.Domain

Namespace Vemar.WPF.Reports

    Public Class BoletaRemedidaReport

        Private Shared _logoBase64 As String = Nothing

        Private Shared Function GetLogoBase64() As String
            If _logoBase64 IsNot Nothing Then Return _logoBase64
            Try
                Dim asm = Assembly.GetExecutingAssembly()
                Using stream = asm.GetManifestResourceStream("Vemar.WPF.logo.png")
                    If stream Is Nothing Then Return String.Empty
                    Dim bytes(CInt(stream.Length) - 1) As Byte
                    stream.Read(bytes, 0, bytes.Length)
                    _logoBase64 = Convert.ToBase64String(bytes)
                End Using
            Catch
                _logoBase64 = String.Empty
            End Try
            Return _logoBase64
        End Function

        Public Async Function GeneratePdfAsync(remedida As Remedida) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rdlcXml = BuildRdlcXml(remedida)
                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using

                    Dim pdfBytes = report.Render("PDF")
                    Dim clave = If(String.IsNullOrWhiteSpace(remedida.ClaveSure), "remedida", remedida.ClaveSure)
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Boleta de Remedida", $"Boleta_Remedida_{clave}_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
                Catch ex As Exception
                    Dim msg = ex.Message
                    Dim inner = ex.InnerException
                    Do While inner IsNot Nothing
                        msg &= " → " & inner.Message
                        inner = inner.InnerException
                    Loop
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar boleta: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        Private Function BuildRdlcXml(r As Remedida) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.65
            Const cW As Double = 7.2   ' content width

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim fechaRem = r.Fecha.ToString("dd/MM/yyyy")
            Dim expediente = If(r.ExpedienteEntregado, "ENTREGADO", "PENDIENTE")
            Dim precio = If(r.Precio > 0D, "L " & r.Precio.ToString("N2", CultureInfo.InvariantCulture), "")

            Dim sb As New StringBuilder()
            sb.Append("<?xml version=""1.0"" encoding=""utf-8""?>")
            sb.Append("<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" ")
            sb.Append("xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">")

            Dim bannerB64 = ReportHeaderHelper.GetBannerBase64()
            sb.Append("<EmbeddedImages>")
            If hasLogo Then
                sb.Append("<EmbeddedImage Name=""VemarLogo"">")
                sb.Append("<MIMEType>image/png</MIMEType>")
                sb.Append($"<ImageData>{logoB64}</ImageData>")
                sb.Append("</EmbeddedImage>")
            End If
            sb.Append("<EmbeddedImage Name=""VemarBanner"">")
            sb.Append("<MIMEType>image/jpeg</MIMEType>")
            sb.Append($"<ImageData>{bannerB64}</ImageData>")
            sb.Append("</EmbeddedImage>")
            sb.Append("</EmbeddedImages>")

            sb.Append("<Body><ReportItems>")

            ' ── Membrete ──────────────────────────────────────────────────────
            Dim hdr = ReportHeaderHelper.BuildHeader(logoB64, cW, "br")
            sb.Append(hdr.xml)

            ' ── Encabezado texto ──
            T(sb, "TxTit", "BOLETA DE RECEPCIÓN — REMEDIDA", FmtIn(hdr.heightUsed + 0.05), "0in", "0.25in", FmtIn(cW), "11pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxGen", $"Generado: {fechaGen}    |    ID: {r.Id}", FmtIn(hdr.heightUsed + 0.30), "0in", "0.18in", FmtIn(cW), "8pt", "Normal", "#64748B", "Left")

            ' ── Cuadrícula de campos ──
            ' Cada campo indica claramente su etiqueta. Solo se muestran los campos
            ' con datos, para no dejar cuadros vacíos sin valor. El estado del
            ' expediente siempre se muestra por ser información relevante.
            Dim gTop = hdr.heightUsed + 0.56
            Dim gRowH = 0.38
            Dim col1W = (cW - 0.1) / 2
            Dim col2X = col1W + 0.1

            Dim allFields As New List(Of (Label As String, Value As String)) From {
                ("Clave SURE", XmlEscOpt(r.ClaveSure)),
                ("Matrícula", XmlEscOpt(r.Matricula)),
                ("Cliente / Propietario", XmlEscOpt(r.Propietario)),
                ("Representante", XmlEscOpt(r.Representante)),
                ("Ubicación", XmlEscOpt(r.Ubicacion)),
                ("CAM", XmlEscOpt(r.Cam)),
                ("Fecha de Contrato", fechaRem),
                ("Precio Contrato", precio),
                ("Expediente", expediente)
            }
            Dim fields = allFields.Where(Function(f) Not String.IsNullOrWhiteSpace(f.Value)).ToList()

            Dim rowIdx As Integer = 0
            Dim fi As Integer = 0
            Do While fi < fields.Count
                Dim rowTop = FmtIn(gTop + (gRowH + 0.06) * rowIdx)
                If fi + 1 < fields.Count Then
                    FieldCell(sb, "F" & fi, fields(fi).Label, fields(fi).Value, rowTop, "0in", FmtIn(col1W), FmtIn(gRowH))
                    FieldCell(sb, "F" & (fi + 1), fields(fi + 1).Label, fields(fi + 1).Value, rowTop, FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))
                    fi += 2
                Else
                    FieldCell(sb, "F" & fi, fields(fi).Label, fields(fi).Value, rowTop, "0in", FmtIn(cW), FmtIn(gRowH))
                    fi += 1
                End If
                rowIdx += 1
            Loop
            Dim gridRows = rowIdx

            ' ── Líneas de firma ──
            Dim sigTop As Double = gTop + (gRowH + 0.06) * gridRows + 0.6
            Dim sigW = cW / 2 - 0.3
            Sep(sb, "SgL", FmtIn(sigTop), FmtIn(sigW), "#374151", "1pt")
            T(sb, "LbSgL", "Firma / Nombre de quien recibe", FmtIn(sigTop + 0.06), "0in", "0.22in", FmtIn(sigW), "8pt", "Normal", "#6B7280", "Center")

            Dim sig2X = cW / 2 + 0.3
            Sep(sb, "SgR", FmtIn(sigTop), FmtIn(cW - sigW), "#374151", "1pt", FmtIn(sig2X))
            T(sb, "LbSgR", "Firma / Nombre de quien entrega", FmtIn(sigTop + 0.06), FmtIn(sig2X), "0.22in", FmtIn(sigW), "8pt", "Normal", "#6B7280", "Center")

            ' ── Pie ──
            Dim footTop = sigTop + 0.5
            Sep(sb, "SFt", FmtIn(footTop), FmtIn(cW), "#E2E8F0", "1pt")
            T(sb, "TxFt", "CONSTRUCTORA VEMAR S. de R.L. de C.V.  —  Documento generado electrónicamente", FmtIn(footTop + 0.06), "0in", "0.2in", FmtIn(cW), "7pt", "Normal", "#94A3B8", "Center")

            Dim brContentH As Double = footTop + 0.26
            Dim brMaxBodyH As Double = pH - 0.50 * 2 - 0.02
            sb.Append($"</ReportItems><Height>{FmtIn(Math.Min(brContentH, brMaxBodyH))}</Height></Body>")
            sb.Append($"<Width>{FmtIn(cW)}</Width>")
            sb.Append("<Page>")
            sb.Append($"<PageHeight>{FmtIn(pH)}</PageHeight><PageWidth>{FmtIn(pW)}</PageWidth>")
            sb.Append($"<LeftMargin>{FmtIn(mg)}</LeftMargin><RightMargin>{FmtIn(mg)}</RightMargin>")
            sb.Append("<TopMargin>0.50in</TopMargin><BottomMargin>0.50in</BottomMargin>")
            sb.Append("</Page><Language>es-HN</Language></Report>")
            Return sb.ToString()
        End Function

        ' ─── Helpers RDLC ────────────────────────────────────────────────
        Private Sub T(sb As StringBuilder, name As String, value As String,
                      top As String, left As String, height As String, width As String,
                      fontSize As String, fontWeight As String, color As String,
                      align As String)
            sb.Append($"<Textbox Name=""{name}""><CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append($"<Value>{value}</Value>")
            sb.Append($"<Style><FontSize>{fontSize}</FontSize><FontWeight>{fontWeight}</FontWeight><Color>{color}</Color></Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append($"<Style><TextAlign>{align}</TextAlign></Style>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("</Textbox>")
        End Sub

        Private Sub Sep(sb As StringBuilder, name As String, top As String, width As String,
                        color As String, thick As String, Optional left As String = "0in")
            sb.Append($"<Rectangle Name=""{name}"">")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>0.02in</Height><Width>{width}</Width>")
            sb.Append($"<Style><BottomBorder><Color>{color}</Color><Style>Solid</Style><Width>{thick}</Width></BottomBorder></Style>")
            sb.Append("</Rectangle>")
        End Sub

        Private Sub BoxRect(sb As StringBuilder, name As String, top As String, left As String,
                            height As String, width As String, borderColor As String)
            sb.Append($"<Rectangle Name=""{name}"">")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append($"<Style><Border><Color>{borderColor}</Color><Style>Solid</Style><Width>1pt</Width></Border>")
            sb.Append("<BackgroundColor>#F8FAFC</BackgroundColor></Style>")
            sb.Append("</Rectangle>")
        End Sub

        ' Dibuja un campo (etiqueta + valor) como UN solo cuadro de texto con dos
        ' párrafos, de modo que la etiqueta y el valor nunca puedan desalinearse.
        Private Sub FieldCell(sb As StringBuilder, name As String, label As String, value As String,
                              top As String, left As String, width As String, height As String)
            sb.Append($"<Textbox Name=""Cl{name}""><CanGrow>true</CanGrow><Paragraphs>")
            sb.Append($"<Paragraph><TextRuns><TextRun><Value>{label}</Value>")
            sb.Append("<Style><FontSize>7pt</FontSize><FontWeight>Bold</FontWeight><Color>#64748B</Color></Style>")
            sb.Append("</TextRun></TextRuns></Paragraph>")
            sb.Append($"<Paragraph><TextRuns><TextRun><Value>{value}</Value>")
            sb.Append("<Style><FontSize>9pt</FontSize><FontWeight>SemiBold</FontWeight><Color>#1F2937</Color></Style>")
            sb.Append("</TextRun></TextRuns></Paragraph>")
            sb.Append("</Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            ' El borde y el fondo van en el propio cuadro de texto (no en un rectángulo
            ' aparte) para evitar el solapamiento que hace que RDLC descarte celdas.
            sb.Append("<Style><Border><Color>#E2E8F0</Color><Style>Solid</Style><Width>1pt</Width></Border>")
            sb.Append("<BackgroundColor>#F8FAFC</BackgroundColor><PaddingLeft>4pt</PaddingLeft><PaddingTop>3pt</PaddingTop></Style>")
            sb.Append("</Textbox>")
        End Sub

        Private Function ShiftIn(inchStr As String, delta As Double) As String
            Dim s = inchStr.Replace("in", "")
            Dim v As Double
            If Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, v) Then
                Return FmtIn(v + delta)
            End If
            Return inchStr
        End Function

        Private Function FmtIn(d As Double) As String
            Return d.ToString("F2", CultureInfo.InvariantCulture) & "in"
        End Function

        Private Function XmlEsc(s As String) As String
            If String.IsNullOrEmpty(s) Then Return "—"
            Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function

        Private Function XmlEscOpt(s As String) As String
            If String.IsNullOrWhiteSpace(s) Then Return ""
            Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function

    End Class

End Namespace
