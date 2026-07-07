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
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Acta de Compromiso del Urbanizador", $"Boleta_Proyecto_{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}")
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

        Private Function BuildRdlcXml(p As Proyecto) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.65
            Const cW As Double = 7.2

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim fechaSolo = DateTime.Now.ToString("dd/MM/yyyy")
            Dim area = p.Area.ToString("N2", CultureInfo.InvariantCulture) & " m²"

            Dim clienteNombre = XmlEsc(If(p.Cliente?.Nombre, "___________________________"))
            Dim clienteRtn = XmlEsc(If(p.Cliente?.Rtn, "___________________________"))
            Dim clienteRep = XmlEsc(If(p.Cliente?.Representante, ""))
            Dim proyNombre = XmlEsc(If(p.Nombre, "—"))
            Dim claveSure = XmlEsc(If(p.ClaveSure, "—"))
            Dim matricula = XmlEsc(If(p.Matricula, "—"))
            Dim ubicacion = XmlEsc(If(p.Ubicacion, "—"))
            Dim categoria = XmlEsc(If(p.CategoriaProyecto?.Nombre, "—"))
            Dim zonif = XmlEsc(If(p.Zonificacion?.Zonificacion, "—"))

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

            ' ── Membrete ──────────────────────────────────────────────────────
            If hasLogo Then
                sb.Append("<Image Name=""ImgLogo""><Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("<Top>0in</Top><Left>0in</Left><Height>0.75in</Height><Width>0.75in</Width><Style/></Image>")
            End If

            Dim hL = FmtIn(If(hasLogo, 0.85, 0.0))
            Dim hW = FmtIn(If(hasLogo, cW - 0.85, cW))

            T(sb, "TxEmp", "CONSTRUCTORA VEMAR S. de R.L. de C.V.", "0.04in", hL, "0.28in", hW, "13pt", "Bold", "#1E3A8A", "Left")
            T(sb, "TxTit", "ACTA DE COMPROMISO DEL URBANIZADOR", "0.34in", hL, "0.25in", hW, "11pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxGen", $"Fecha: {fechaGen}    |    Registro No. {p.Id}", "0.61in", hL, "0.18in", hW, "8pt", "Normal", "#64748B", "Left")

            Sep(sb, "S0", "0.86in", FmtIn(cW), "#1E40AF", "2pt")

            ' ── Nombre del proyecto ──
            Dim nTop = 0.98
            BoxRect(sb, "BxNom", FmtIn(nTop), "0in", "0.72in", FmtIn(cW), "#1E40AF")
            T(sb, "LbNom", "NOMBRE DEL PROYECTO", FmtIn(nTop + 0.06), "0.12in", "0.2in", FmtIn(cW - 0.2), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlNom", proyNombre, FmtIn(nTop + 0.28), "0.1in", "0.35in", FmtIn(cW - 0.15), "14pt", "Bold", "#1E3A8A", "Left")

            ' ── Clave SURE | Matrícula ──
            Dim idTop = nTop + 0.72 + 0.10
            Dim halfW = (cW - 0.1) / 2
            BoxRect(sb, "BxCS", FmtIn(idTop), "0in", "0.62in", FmtIn(halfW), "#1E40AF")
            T(sb, "LbCS", "CLAVE SURE", FmtIn(idTop + 0.06), "0.1in", "0.2in", FmtIn(halfW - 0.15), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlCS", claveSure, FmtIn(idTop + 0.27), "0.1in", "0.28in", FmtIn(halfW - 0.15), "13pt", "Bold", "#1E3A8A", "Left")

            Dim rx = halfW + 0.1
            BoxRect(sb, "BxMt", FmtIn(idTop), FmtIn(rx), "0.62in", FmtIn(halfW), "#1E40AF")
            T(sb, "LbMt", "MATRÍCULA", FmtIn(idTop + 0.06), FmtIn(rx + 0.1), "0.2in", FmtIn(halfW - 0.15), "8pt", "Bold", "#94A3B8", "Left")
            T(sb, "VlMt", matricula, FmtIn(idTop + 0.27), FmtIn(rx + 0.1), "0.28in", FmtIn(halfW - 0.15), "13pt", "Bold", "#1E3A8A", "Left")

            ' ── Cuadrícula de campos ──
            Dim gTop = idTop + 0.62 + 0.12
            Dim gRowH = 0.38
            Dim col1W = (cW - 0.1) / 2
            Dim col2X = col1W + 0.1

            ' Fila 1: Nombre del proyecto (ancho completo)
            BoxRect(sb, "BxNP", FmtIn(gTop), "0in", FmtIn(gRowH), FmtIn(cW), "#FFFFFF")
            T(sb, "LbNP", "Nombre del Proyecto", FmtIn(gTop + 0.05), "0.08in", "0.14in", FmtIn(cW - 0.16), "7pt", "Normal", "#64748B", "Left")
            T(sb, "VlNP", proyNombre, FmtIn(gTop + 0.18), "0.08in", "0.18in", FmtIn(cW - 0.16), "11pt", "Bold", "#0F172A", "Left")

            FieldPair(sb, "Cl", "Cliente / Urbanizador", clienteNombre,
                          "Rtn", "R.T.N.", clienteRtn,
                          FmtIn(gTop + gRowH + 0.06), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            FieldPair(sb, "Rep", "Representante Legal", clienteRep,
                          "Tel", "Teléfono", XmlEsc(If(p.Cliente?.Telefono, "—")),
                          FmtIn(gTop + (gRowH + 0.06) * 2), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            FieldPair(sb, "Cat", "Categoría", categoria,
                          "Zon", "Zonificación", zonif,
                          FmtIn(gTop + (gRowH + 0.06) * 3), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            FieldPair(sb, "Ar", "Área", area,
                          "Ub", "Ubicación", ubicacion,
                          FmtIn(gTop + (gRowH + 0.06) * 4), "0in", FmtIn(col1W), FmtIn(col2X), FmtIn(col1W), FmtIn(gRowH))

            ' ── Texto de compromiso ──
            Dim cmpTop = gTop + (gRowH + 0.06) * 5 + 0.18
            Dim cmpText =
                "Yo, el/la urbanizador(a) identificado(a) en este documento, declaro y acepto expresamente " &
                "que los datos del proyecto indicados en la presente acta son correctos y corresponden al " &
                "inmueble de mi propiedad o bajo mi representación. Asimismo, me comprometo a cumplir con " &
                "todos los trámites, requisitos y obligaciones establecidos por CONSTRUCTORA VEMAR S. de R.L. de C.V. " &
                "para la tramitación del proyecto. La suscripción de este documento con mi firma y huella dactilar " &
                "constituye plena conformidad con la información aquí consignada y no podrá ser objeto de " &
                "desconocimiento posterior."
            BoxRect(sb, "BxCmp", FmtIn(cmpTop), "0in", "1.05in", FmtIn(cW), "#CBD5E1")
            T(sb, "TxCmpT", "DECLARACIÓN Y COMPROMISO", FmtIn(cmpTop + 0.06), "0.12in", "0.20in", FmtIn(cW - 0.24), "7pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxCmpB", cmpText, FmtIn(cmpTop + 0.28), "0.12in", "0.72in", FmtIn(cW - 0.24), "8pt", "Normal", "#374151", "Left")

            ' ── Sección de firma y huella ──
            Dim sigTop = cmpTop + 1.05 + 0.30

            ' Columna izquierda — Firma del urbanizador
            Dim sigColW = cW / 2 - 0.20
            Sep(sb, "SgFirmaUrb", FmtIn(sigTop + 0.55), FmtIn(sigColW), "#374151", "1pt")
            T(sb, "LbFirmaUrb", "Firma del Urbanizador", FmtIn(sigTop + 0.63), "0in", "0.18in", FmtIn(sigColW), "7pt", "Normal", "#6B7280", "Center")
            Sep(sb, "SgNomUrb", FmtIn(sigTop + 0.95), FmtIn(sigColW), "#374151", "1pt")
            T(sb, "LbNomUrb", "Nombre completo", FmtIn(sigTop + 1.03), "0in", "0.18in", FmtIn(sigColW), "7pt", "Normal", "#6B7280", "Center")
            Sep(sb, "SgDniUrb", FmtIn(sigTop + 1.35), FmtIn(sigColW), "#374151", "1pt")
            T(sb, "LbDniUrb", "No. Identidad / RTN", FmtIn(sigTop + 1.43), "0in", "0.18in", FmtIn(sigColW), "7pt", "Normal", "#6B7280", "Center")

            ' Columna derecha — Huella dactilar + Firma Vemar
            Dim rCol = sigColW + 0.40
            Dim huellaW = 1.10
            Dim huellaH = 1.30
            BoxRect(sb, "BxHuella", FmtIn(sigTop), FmtIn(rCol), FmtIn(huellaH), FmtIn(huellaW), "#94A3B8")
            T(sb, "LbHuella1", "HUELLA", FmtIn(sigTop + 0.52), FmtIn(rCol), "0.20in", FmtIn(huellaW), "8pt", "Bold", "#94A3B8", "Center")
            T(sb, "LbHuella2", "DACTILAR", FmtIn(sigTop + 0.72), FmtIn(rCol), "0.20in", FmtIn(huellaW), "8pt", "Bold", "#94A3B8", "Center")

            Dim vemarX = rCol + huellaW + 0.25
            Dim vemarW = cW - vemarX
            Sep(sb, "SgFirmaVmr", FmtIn(sigTop + 0.55), FmtIn(vemarW), "#374151", "1pt", FmtIn(vemarX))
            T(sb, "LbFirmaVmr", "Firma Vemar", FmtIn(sigTop + 0.63), FmtIn(vemarX), "0.18in", FmtIn(vemarW), "7pt", "Normal", "#6B7280", "Center")
            T(sb, "TxFecha", $"Comayagua, {fechaSolo}", FmtIn(sigTop + 1.30), FmtIn(vemarX), "0.18in", FmtIn(vemarW), "7pt", "Normal", "#374151", "Center")

            ' ── Pie ──
            Dim footTop = sigTop + 1.70
            Sep(sb, "SFt", FmtIn(footTop), FmtIn(cW), "#E2E8F0", "1pt")
            T(sb, "TxFt", "CONSTRUCTORA VEMAR S. de R.L. de C.V.  —  Documento generado electrónicamente  —  Original para archivo",
              FmtIn(footTop + 0.06), "0in", "0.2in", FmtIn(cW), "7pt", "Normal", "#94A3B8", "Center")

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
