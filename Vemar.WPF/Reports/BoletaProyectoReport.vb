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

    Public Class BoletaProyectoReport

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

        Public Async Function GeneratePdfAsync(proyecto As Proyecto) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rdlcXml = BuildRdlcXml(proyecto)
                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using

                    Dim pdfBytes = report.Render("PDF")
                    Dim nombre = If(String.IsNullOrWhiteSpace(proyecto.Nombre), "proyecto", proyecto.Nombre)
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"Boleta_Proyecto_{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf")
                    File.WriteAllBytes(filePath, pdfBytes)
                    Process.Start(New ProcessStartInfo With {.FileName = filePath, .UseShellExecute = True})
                    Return filePath
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

        Private Function BuildRdlcXml(p As Proyecto) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.65
            Const cW As Double = 7.2

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim area = p.Area.ToString("N2", CultureInfo.InvariantCulture) & " m²"

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

            If hasLogo Then
                sb.Append("<Image Name=""ImgLogo""><Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("<Top>0in</Top><Left>0in</Left><Height>0.75in</Height><Width>0.75in</Width><Style/></Image>")
            End If

            Dim hL = FmtIn(If(hasLogo, 0.85, 0.0))
            Dim hW = FmtIn(If(hasLogo, cW - 0.85, cW))

            T(sb, "TxEmp", "CONSTRUCTORA VEMAR S. de R.L. de C.V.", "0.04in", hL, "0.28in", hW, "13pt", "Bold", "#1E3A8A", "Left")
            T(sb, "TxTit", "BOLETA DE PROYECTO", "0.34in", hL, "0.25in", hW, "11pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxGen", $"Generado: {fechaGen}    |    ID: {p.Id}", "0.61in", hL, "0.18in", hW, "8pt", "Normal", "#64748B", "Left")

            Sep(sb, "S0", "0.86in", FmtIn(cW), "#1E40AF", "2pt")

            ' ── Nombre del proyecto (caja principal destacada) ──
            Dim nTop = 0.98
            BoxRect(sb, "BxNom", FmtIn(nTop), "0in", "0.72in", FmtIn(cW), "#1E40AF")
            T(sb, "LbNom", "PROYECTO", FmtIn(nTop + 0.06), "0.12in", "0.2in", FmtIn(cW - 0.2), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlNom", XmlEsc(p.Nombre), FmtIn(nTop + 0.28), "0.1in", "0.35in", FmtIn(cW - 0.15), "14pt", "Bold", "#1E3A8A", "Left")

            ' ── Clave SURE | Matrícula ──
            Dim idTop = nTop + 0.72 + 0.1
            Dim halfW = (cW - 0.1) / 2
            BoxRect(sb, "BxCS", FmtIn(idTop), "0in", "0.62in", FmtIn(halfW), "#1E40AF")
            T(sb, "LbCS", "CLAVE SURE", FmtIn(idTop + 0.06), "0.1in", "0.2in", FmtIn(halfW - 0.15), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlCS", XmlEsc(p.ClaveSure), FmtIn(idTop + 0.27), "0.1in", "0.28in", FmtIn(halfW - 0.15), "13pt", "Bold", "#1E3A8A", "Left")

            Dim rx = halfW + 0.1
            BoxRect(sb, "BxMt", FmtIn(idTop), FmtIn(rx), "0.62in", FmtIn(halfW), "#1E40AF")
            T(sb, "LbMt", "MATRÍCULA", FmtIn(idTop + 0.06), FmtIn(rx + 0.1), "0.2in", FmtIn(halfW - 0.15), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlMt", XmlEsc(p.Matricula), FmtIn(idTop + 0.27), FmtIn(rx + 0.1), "0.28in", FmtIn(halfW - 0.15), "13pt", "Bold", "#1E3A8A", "Left")

            ' ── Cuadrícula de campos ──
            Dim gTop = idTop + 0.62 + 0.12
            Dim gRowH = 0.38
            Dim col1W = (cW - 0.1) / 2
            Dim col2X = col1W + 0.1

            ' Fila 1: Cliente | RTN
            Dim rtnCliente = If(p.Cliente IsNot Nothing, "RTN: " & If(p.Cliente.Rtn, "—"), "—")
            FieldPair(sb, "Cl", "Cliente", XmlEsc(If(p.Cliente?.Nombre, "—")),
                          "Rtn", "RTN Cliente", XmlEsc(If(p.Cliente?.Rtn, "—")),
                          FmtIn(gTop), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' Fila 2: Representante cliente | Email
            FieldPair(sb, "Rep", "Representante", XmlEsc(If(p.Cliente?.Representante, "—")),
                          "Tel", "Teléfono", XmlEsc(If(p.Cliente?.Telefono, "—")),
                          FmtIn(gTop + gRowH + 0.06), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' Fila 3: Categoría | Zonificación
            FieldPair(sb, "Cat", "Categoría", XmlEsc(If(p.CategoriaProyecto?.Nombre, "—")),
                          "Zon", "Zonificación", XmlEsc(If(p.Zonificacion?.Zonificacion, "—")),
                          FmtIn(gTop + (gRowH + 0.06) * 2), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' Fila 4: Área | Ubicación
            FieldPair(sb, "Ar", "Área", area,
                          "Ub", "Ubicación", XmlEsc(p.Ubicacion),
                          FmtIn(gTop + (gRowH + 0.06) * 3), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' ── Líneas de firma ──
            Dim sigTop = gTop + (gRowH + 0.06) * 4 + 0.45
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

        Private Sub T(sb As StringBuilder, name As String, value As String,
                      top As String, left As String, height As String, width As String,
                      fontSize As String, fontWeight As String, color As String, align As String)
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
            BoxRect(sb, "Bx" & n1, top, left1, height, width1, "#E2E8F0")
            T(sb, "Lb" & n1, lbl1, top, left1, "0.18in", width1, "7pt", "Bold", "#64748B", "Left")
            T(sb, "Vl" & n1, val1, ShiftIn(top, 0.2), left1, ShiftIn(height, -0.2), width1, "9pt", "SemiBold", "#1F2937", "Left")
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
