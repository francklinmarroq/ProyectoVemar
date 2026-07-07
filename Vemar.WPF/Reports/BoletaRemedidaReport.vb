Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
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
            Dim expediente = If(r.ExpedienteEntregado, "✔  ENTREGADO", "✘  PENDIENTE")
            Dim precio = "L " & r.Precio.ToString("N2", CultureInfo.InvariantCulture)

            Dim sb As New StringBuilder()
            sb.Append("<?xml version=""1.0"" encoding=""utf-8""?>")
            sb.Append("<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" ")
            sb.Append("xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">")

            If hasLogo Then
                sb.Append("<EmbeddedImages><EmbeddedImage Name=""VemarLogo"">")
                sb.Append("<MIMEType>image/png</MIMEType>")
                sb.Append($"<ImageData>{logoB64}</ImageData>")
                sb.Append("</EmbeddedImage></EmbeddedImages>")
            End If

            sb.Append("<Body><ReportItems>")

            ' ── Logo ──
            If hasLogo Then
                sb.Append("<Image Name=""ImgLogo""><Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("<Top>0in</Top><Left>0in</Left><Height>0.75in</Height><Width>0.75in</Width><Style/></Image>")
            End If

            Dim hL = FmtIn(If(hasLogo, 0.85, 0.0))
            Dim hW = FmtIn(If(hasLogo, cW - 0.85, cW))

            ' ── Encabezado texto ──
            T(sb, "TxEmp", "CONSTRUCTORA VEMAR S. de R.L. de C.V.", "0.04in", hL, "0.28in", hW, "13pt", "Bold", "#1E3A8A", "Left")
            T(sb, "TxTit", "BOLETA DE RECEPCIÓN — REMEDIDA", "0.34in", hL, "0.25in", hW, "11pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxGen", $"Generado: {fechaGen}    |    ID: {r.Id}", "0.61in", hL, "0.18in", hW, "8pt", "Normal", "#64748B", "Left")

            ' Línea divisora encabezado
            Sep(sb, "S0", "0.86in", FmtIn(cW), "#1E40AF", "2pt")

            ' ── Caja identificadora grande (CLAVE SURE | MATRÍCULA) ──
            Dim bTop = 0.98
            Dim bH = 0.72
            Dim halfW = (cW - 0.1) / 2

            BoxRect(sb, "BxL", FmtIn(bTop), "0in", FmtIn(bH), FmtIn(halfW), "#1E40AF")
            T(sb, "LbCS", "CLAVE SURE", FmtIn(bTop + 0.06), "0.12in", "0.2in", FmtIn(halfW - 0.2), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlCS", XmlEsc(r.ClaveSure), FmtIn(bTop + 0.28), "0.1in", "0.3in", FmtIn(halfW - 0.15), "15pt", "Bold", "#1E3A8A", "Left")

            Dim rx = halfW + 0.1
            BoxRect(sb, "BxR", FmtIn(bTop), FmtIn(rx), FmtIn(bH), FmtIn(halfW), "#1E40AF")
            T(sb, "LbMt", "MATRÍCULA", FmtIn(bTop + 0.06), FmtIn(rx + 0.12), "0.2in", FmtIn(halfW - 0.2), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlMt", XmlEsc(r.Matricula), FmtIn(bTop + 0.28), FmtIn(rx + 0.1), "0.3in", FmtIn(halfW - 0.15), "15pt", "Bold", "#1E3A8A", "Left")

            ' ── Campos en cuadrícula 2×3 ──
            Dim gTop = bTop + bH + 0.12
            Dim gRowH = 0.38
            Dim col1W = (cW - 0.1) / 2
            Dim col2X = col1W + 0.1

            ' Fila 1: Representante | Fecha
            FieldPair(sb, "Rep", "Representante", XmlEsc(r.Representante),
                          "Fch", "Fecha de Contrato", fechaRem,
                          FmtIn(gTop), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' Fila 2: Ubicación | Precio
            FieldPair(sb, "Ub", "Ubicación", XmlEsc(r.Ubicacion),
                          "Pr", "Precio Contrato", precio,
                          FmtIn(gTop + gRowH + 0.06), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' Fila 3: CAM | Expediente
            FieldPair(sb, "Cm", "CAM", XmlEsc(r.Cam),
                          "Ex", "Expediente", expediente,
                          FmtIn(gTop + (gRowH + 0.06) * 2), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' ── Objeto ──
            Dim objTop = gTop + (gRowH + 0.06) * 3 + 0.06
            Sep(sb, "S1", FmtIn(objTop - 0.04), FmtIn(cW), "#CBD5E1", "1pt")
            T(sb, "LbObj", "OBJETO", FmtIn(objTop), "0in", "0.2in", FmtIn(cW), "8pt", "Bold", "#64748B", "Left")
            BoxRect(sb, "BxObj", FmtIn(objTop + 0.22), "0in", "1.4in", FmtIn(cW), "#CBD5E1")
            T(sb, "VlObj", XmlEsc(r.Objeto), FmtIn(objTop + 0.28), "0.1in", "1.3in", FmtIn(cW - 0.15), "9pt", "Normal", "#1F2937", "Left")

            ' ── Líneas de firma ──
            Dim sigTop = objTop + 0.22 + 1.4 + 0.45
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

            sb.Append("</ReportItems><Height>10in</Height></Body>")
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

        Private Sub FieldPair(sb As StringBuilder,
                              n1 As String, lbl1 As String, val1 As String,
                              n2 As String, lbl2 As String, val2 As String,
                              top As String, left1 As String, width1 As String,
                              left2 As String, width2 As String, height As String)
            ' Caja izquierda
            BoxRect(sb, "Bx" & n1, top, left1, height, width1, "#E2E8F0")
            T(sb, "Lb" & n1, lbl1, top, left1, "0.18in", width1, "7pt", "Bold", "#64748B", "Left")
            T(sb, "Vl" & n1, val1, ShiftIn(top, 0.2), left1, ShiftIn(height, -0.2), width1, "9pt", "SemiBold", "#1F2937", "Left")
            ' Caja derecha
            BoxRect(sb, "Bx" & n2, top, left2, height, width2, "#E2E8F0")
            T(sb, "Lb" & n2, lbl2, top, left2, "0.18in", width2, "7pt", "Bold", "#64748B", "Left")
            T(sb, "Vl" & n2, val2, ShiftIn(top, 0.2), left2, ShiftIn(height, -0.2), width2, "9pt", "SemiBold", "#1F2937", "Left")
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

    End Class

End Namespace
