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

    Public Class ReciboPagoContratoReport

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

        Public Async Function GeneratePdfAsync(pago As PagoContrato, contrato As Contrato,
                                               Optional saldoAnterior As Decimal = -1,
                                               Optional saldoActual As Decimal = -1) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rdlcXml = BuildRdlcXml(pago, contrato, saldoAnterior, saldoActual)
                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using

                    Dim pdfBytes = report.Render("PDF")
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"Recibo_Pago_{pago.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf")
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
                        MessageBox.Show("Error al generar recibo: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        Private Function BuildRdlcXml(pago As PagoContrato, contrato As Contrato,
                                      saldoAnterior As Decimal, saldoActual As Decimal) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.65
            Const cW As Double = 7.2   ' ancho del contenido

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim fecha = DateTime.Now.ToString("dd/MM/yyyy")
            Dim montoNum = pago.Valor.ToString("N2", CultureInfo.InvariantCulture)
            Dim montoStr = "L. " & montoNum

            Dim contratistaNombre As String = If(contrato.Contratista IsNot Nothing AndAlso
                                                 Not String.IsNullOrWhiteSpace(contrato.Contratista.Nombre),
                                                 contrato.Contratista.Nombre, "N/D")
            Dim proyectoNombre As String = If(contrato.Proyecto IsNot Nothing AndAlso
                                              Not String.IsNullOrWhiteSpace(contrato.Proyecto.Nombre),
                                              contrato.Proyecto.Nombre, "N/D")
            Dim concepto As String
            If Not String.IsNullOrWhiteSpace(pago.Descripcion) Then
                concepto = pago.Descripcion
            ElseIf Not String.IsNullOrWhiteSpace(contrato.Descripcion) Then
                concepto = contrato.Descripcion
            Else
                concepto = "Pago de contrato"
            End If

            Dim sFmt = Function(d As Decimal) "L. " & d.ToString("N2", CultureInfo.InvariantCulture)
            Dim sAntStr = If(saldoAnterior >= 0, sFmt(saldoAnterior), "")
            Dim sActStr = If(saldoActual >= 0, sFmt(saldoActual), "")

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

            ' ── Membrete ────────────────────────────────────────────────────
            If hasLogo Then
                sb.Append("<Image Name=""ImgLogo""><Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("<Top>0in</Top><Left>0in</Left><Height>0.80in</Height><Width>0.80in</Width><Style/></Image>")
            End If

            Dim nL = FmtIn(If(hasLogo, 0.90, 0.0))
            Dim nW = FmtIn(3.80)   ' ancho del bloque de nombre/dirección

            T(sb, "TxEmp",  "CONSTRUCTORA VEMAR S. DE R.L. DE C.V.", "0.02in", nL, "0.28in", nW, "11pt", "Bold",   "#1E3A8A", "Left")
            T(sb, "TxAdr",  "Bo. Abajo, Calle Principal, Comayagua, Honduras C.A.",   "0.32in", nL, "0.20in", nW, "8pt",  "Normal", "#374151", "Left")
            T(sb, "TxCtc",  "Cel: +504 9865-3381    R.T.N. 03019012468535",            "0.52in", nL, "0.18in", nW, "8pt",  "Normal", "#374151", "Left")

            ' Bloque "Por LPS." + cuadro de monto (derecha del membrete)
            Dim amtLeft = 5.10
            T(sb,   "TxPorLps", "Por LPS.",  "0.10in", FmtIn(amtLeft),        "0.20in", "0.72in", "9pt", "Bold",   "#374151", "Left")
            TBg(sb, "BxMonto",  Sanitize(montoNum), "0.05in", FmtIn(amtLeft + 0.75), "0.28in", "1.35in",
                "9pt", "Bold", "#FFFBEB", "#D97706", "Center", "#92400E")

            ' Fecha (esquina derecha)
            T(sb,   "TxFchLbl", "Fecha:", "0.52in", FmtIn(amtLeft),        "0.20in", "0.55in", "8pt", "Normal", "#374151", "Left")
            TUnder(sb, "TxFcha", fecha,   "0.52in", FmtIn(amtLeft + 0.58), "0.20in", FmtIn(cW - amtLeft - 0.58))

            Sep(sb, "S0", "0.88in", FmtIn(cW), "#1E3A8A", "2pt")

            ' ── Líneas de formulario ─────────────────────────────────────────
            Dim y = 1.02

            ' Recibí de: (siempre la razón social de Vemar)
            T(sb,      "LbRec",  "Recib" & Chr(237) & " de:", FmtIn(y), "0in",          "0.22in", "1.05in", "9pt", "Bold",   "#374151", "Left")
            TUnder(sb, "VlRec",  "CONSTRUCTORA VEMAR S. DE R.L. DE C.V.", FmtIn(y), "1.05in", "0.22in", FmtIn(cW - 1.05))
            y += 0.36

            ' La Cantidad de:
            T(sb,      "LbCant", "La Cantidad de:",           FmtIn(y), "0in",           "0.22in", "1.38in", "9pt", "Bold",   "#374151", "Left")
            TUnder(sb, "VlCant", Sanitize(montoStr),          FmtIn(y), "1.38in",        "0.22in", FmtIn(cW - 1.38 - 0.72))
            T(sb,      "TxLmp",  "Lempiras",                  FmtIn(y), FmtIn(cW - 0.7),"0.22in", "0.7in",  "9pt", "Normal", "#374151", "Right")
            y += 0.36

            ' Por Concepto de:
            T(sb,      "LbConc", "Por Concepto de:",          FmtIn(y), "0in",           "0.22in", "1.50in", "9pt", "Bold",   "#374151", "Left")
            TUnder(sb, "VlCnc1", Sanitize(concepto),          FmtIn(y), "1.50in",        "0.22in", FmtIn(cW - 1.50))
            y += 0.34
            TUnder(sb, "VlCnc2", "",                          FmtIn(y), "0in",           "0.22in", FmtIn(cW))
            y += 0.34

            Sep(sb, "S1", FmtIn(y), FmtIn(cW), "#CBD5E1", "1pt")
            y += 0.14

            ' ── Fila inferior: Firma | Recibo Interno | Tabla Saldos ──────────
            '  — Firma —
            Dim firmaY = y + 0.55
            Sep(sb, "SgFirma", FmtIn(firmaY), "2.60in", "#374151", "1pt")
            T(sb, "LbFirma", "FIRMA", FmtIn(firmaY + 0.06), "0.70in", "0.22in", "1.20in", "9pt", "Bold", "#374151", "Center")

            '  — Recibo Interno No. —
            Dim riLeft = 2.75
            Dim riW = 1.70
            T(sb, "TxRiLbl", "Recibo Interno",         FmtIn(y + 0.15), FmtIn(riLeft), "0.24in", FmtIn(riW), "9pt",  "Bold",   "#D97706", "Center")
            T(sb, "TxRiNum", "No. " & pago.Id.ToString(), FmtIn(y + 0.42), FmtIn(riLeft), "0.28in", FmtIn(riW), "12pt", "Bold",   "#D97706", "Center")

            '  — Tabla Saldos —
            Dim tL = 4.65                ' left de la tabla
            Dim cLbl = 1.55              ' ancho columna etiqueta
            Dim cVal = 0.50              ' ancho columna valor  (x2)
            Dim rH = 0.24
            Dim tY = y

            ' Encabezado de columnas
            TBg(sb, "ThLbl", "",    FmtIn(tY), FmtIn(tL),            FmtIn(rH), FmtIn(cLbl), "7pt", "Bold", "#E2E8F0", "#94A3B8", "Center", "#374151")
            TBg(sb, "ThV1",  "L.", FmtIn(tY), FmtIn(tL + cLbl),     FmtIn(rH), FmtIn(cVal), "7pt", "Bold", "#E2E8F0", "#94A3B8", "Center", "#374151")
            TBg(sb, "ThV2",  "",   FmtIn(tY), FmtIn(tL + cLbl + cVal), FmtIn(rH), FmtIn(cVal), "7pt", "Bold", "#E2E8F0", "#94A3B8", "Center", "#374151")
            tY += rH

            TBg(sb, "Tr2L", "Abono             L.", FmtIn(tY), FmtIn(tL),                FmtIn(rH), FmtIn(cLbl), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Left",   "#374151")
            TBg(sb, "Tr2V", Sanitize(montoStr),     FmtIn(tY), FmtIn(tL + cLbl),         FmtIn(rH), FmtIn(cVal), "7pt", "Normal", "#FFFBEB", "#CBD5E1", "Center", "#92400E")
            TBg(sb, "Tr2X", "",                     FmtIn(tY), FmtIn(tL + cLbl + cVal),  FmtIn(rH), FmtIn(cVal), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Center", "#374151")
            tY += rH


            ' ── Pie ─────────────────────────────────────────────────────────
            Dim ftY = firmaY + 0.42
            Sep(sb, "SFt", FmtIn(ftY), FmtIn(cW), "#E2E8F0", "1pt")
            T(sb, "TxFt", "CONSTRUCTORA VEMAR S. DE R.L. DE C.V. - Documento generado electronicamente",
              FmtIn(ftY + 0.06), "0in", "0.18in", FmtIn(cW), "7pt", "Normal", "#94A3B8", "Center")

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

        ''' <summary>Textbox plano sin fondo ni borde.</summary>
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

        ''' <summary>Textbox con fondo y borde integrados — SIN rectángulo superpuesto.</summary>
        Private Sub TBg(sb As StringBuilder, name As String, value As String,
                        top As String, left As String, height As String, width As String,
                        fontSize As String, fontWeight As String,
                        bgColor As String, borderColor As String, align As String, textColor As String)
            sb.Append($"<Textbox Name=""{name}""><CanGrow>false</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append($"<Value>{value}</Value>")
            sb.Append($"<Style><FontSize>{fontSize}</FontSize><FontWeight>{fontWeight}</FontWeight><Color>{textColor}</Color></Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append($"<Style><TextAlign>{align}</TextAlign></Style>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("<Style>")
            sb.Append($"<Border><Color>{borderColor}</Color><Style>Solid</Style><Width>1pt</Width></Border>")
            sb.Append($"<BackgroundColor>{bgColor}</BackgroundColor>")
            sb.Append("<PaddingLeft>6pt</PaddingLeft><PaddingTop>3pt</PaddingTop>")
            sb.Append("<PaddingRight>4pt</PaddingRight><PaddingBottom>3pt</PaddingBottom>")
            sb.Append("</Style>")
            sb.Append("</Textbox>")
        End Sub

        ''' <summary>Campo de datos: etiqueta pequeña arriba + valor debajo, borde gris claro.</summary>
        Private Sub TField(sb As StringBuilder, name As String, label As String, value As String,
                           top As String, left As String, height As String, width As String)
            ' Fila de etiqueta (alto fijo 0.18in)
            TBg(sb, name & "L", label, top, left, "0.18in", width,
                "7pt", "Bold", "#F1F5F9", "#CBD5E1", "Left", "#64748B")
            ' Fila de valor (resto del alto)
            Dim valH = SubIn(height, 0.18)
            Dim valTop = AddIn(top, 0.18)
            TBg(sb, name & "V", value, valTop, left, valH, width,
                "9pt", "Normal", "#FFFFFF", "#CBD5E1", "Left", "#1F2937")
        End Sub

        ''' <summary>Textbox con solo borde inferior — estilo línea de formulario.</summary>
        Private Sub TUnder(sb As StringBuilder, name As String, value As String,
                           top As String, left As String, height As String, width As String,
                           Optional fontSize As String = "9pt", Optional textColor As String = "#1F2937")
            sb.Append($"<Textbox Name=""{name}""><CanGrow>false</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append($"<Value>{value}</Value>")
            sb.Append($"<Style><FontSize>{fontSize}</FontSize><FontWeight>Normal</FontWeight><Color>{textColor}</Color></Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append("<Style><TextAlign>Left</TextAlign></Style>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("<Style>")
            sb.Append("<Border><Style>None</Style></Border>")
            sb.Append("<BottomBorder><Color>#374151</Color><Style>Solid</Style><Width>1pt</Width></BottomBorder>")
            sb.Append("<PaddingLeft>4pt</PaddingLeft><PaddingBottom>2pt</PaddingBottom>")
            sb.Append("</Style>")
            sb.Append("</Textbox>")
        End Sub

        ''' <summary>Línea separadora delgada.</summary>
        Private Sub Sep(sb As StringBuilder, name As String, top As String, width As String,
                        color As String, thick As String, Optional left As String = "0in")
            sb.Append($"<Rectangle Name=""{name}"">")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>0.02in</Height><Width>{width}</Width>")
            sb.Append($"<Style><BottomBorder><Color>{color}</Color><Style>Solid</Style><Width>{thick}</Width></BottomBorder></Style>")
            sb.Append("</Rectangle>")
        End Sub

        Private Function FmtIn(d As Double) As String
            Return d.ToString("F2", CultureInfo.InvariantCulture) & "in"
        End Function

        Private Function AddIn(inchStr As String, delta As Double) As String
            Dim v As Double
            If Double.TryParse(inchStr.Replace("in", ""), NumberStyles.Any, CultureInfo.InvariantCulture, v) Then
                Return FmtIn(v + delta)
            End If
            Return inchStr
        End Function

        Private Function SubIn(inchStr As String, delta As Double) As String
            Return AddIn(inchStr, -delta)
        End Function

        ''' <summary>Escapa XML y elimina caracteres de control no válidos.</summary>
        Private Function Sanitize(s As String) As String
            If String.IsNullOrEmpty(s) Then Return "N/D"
            ' Eliminar caracteres de control (excepto tab/LF/CR que son válidos en XML)
            Dim clean As New StringBuilder(s.Length)
            For Each c As Char In s
                Dim code = AscW(c)
                If code = 9 OrElse code = 10 OrElse code = 13 OrElse (code >= 32 AndAlso code <> 65535) Then
                    clean.Append(c)
                End If
            Next
            Dim r = clean.ToString()
            If String.IsNullOrWhiteSpace(r) Then Return "N/D"
            Return r.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function

    End Class

End Namespace
