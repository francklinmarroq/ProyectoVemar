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

    Public Class ReciboCobroRemedidaReport

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

        Public Async Function GeneratePdfAsync(cobro As CobroRemedida, remedida As Remedida, Optional totalAbonado As Decimal? = Nothing) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rdlcXml = BuildRdlcXml(cobro, remedida, If(totalAbonado, cobro.Cantidad))
                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using
                    Dim pdfBytes = report.Render("PDF")
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Recibo de Cobro", $"Recibo_Cobro_{cobro.Id}_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
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

        Private Function BuildRdlcXml(cobro As CobroRemedida, remedida As Remedida, totalAbonado As Decimal) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.65
            Const cW As Double = 7.2

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim fecha = DateTime.Now.ToString("dd/MM/yyyy")
            Dim montoNum = cobro.Cantidad.ToString("N2", CultureInfo.InvariantCulture)
            Dim montoStr = "L. " & montoNum
            Dim centavos = CInt(Math.Round((cobro.Cantidad - Math.Floor(cobro.Cantidad)) * 100))
            Dim montoLetras = NumerosALetras(Math.Floor(cobro.Cantidad)) & " CON " & centavos.ToString("00") & "/100"

            Dim precioTotal = remedida.Precio
            Dim saldo = precioTotal - totalAbonado
            Dim precioTotalStr = "L. " & precioTotal.ToString("N2", CultureInfo.InvariantCulture)
            Dim abonoStr = "L. " & totalAbonado.ToString("N2", CultureInfo.InvariantCulture)
            Dim saldoStr = "L. " & saldo.ToString("N2", CultureInfo.InvariantCulture)

            Dim propietario As String = If(Not String.IsNullOrWhiteSpace(remedida.Propietario),
                                           remedida.Propietario,
                                           If(Not String.IsNullOrWhiteSpace(remedida.Representante),
                                              remedida.Representante, "N/D"))

            Dim concepto As String = "Cobro de Remedida"
            If Not String.IsNullOrWhiteSpace(remedida.ClaveSure) Then
                concepto &= " — Clave SURE: " & remedida.ClaveSure
            End If
            If Not String.IsNullOrWhiteSpace(remedida.Matricula) Then
                concepto &= " / Matrícula: " & remedida.Matricula
            End If
            If Not String.IsNullOrWhiteSpace(remedida.Ubicacion) Then
                concepto &= " / " & remedida.Ubicacion
            End If

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

            ' ── Membrete ────────────────────────────────────────────────────
            Dim hdr = ReportHeaderHelper.BuildHeader(logoB64, cW, "rcr")
            sb.Append(hdr.xml)

            Dim amtLeft = 4.90
            T(sb, "TxPorLps", "Por LPS.", "0.10in", FmtIn(amtLeft), "0.20in", "0.72in", "9pt", "Bold", "#374151", "Left")
            TBg(sb, "BxMonto", Sanitize(montoNum), "0.05in", FmtIn(amtLeft + 0.75), "0.28in", "1.45in",
                "9pt", "Bold", "#FFFBEB", "#D97706", "Center", "#92400E")

            T(sb, "TxFchLbl", "Fecha:", "0.52in", FmtIn(amtLeft), "0.20in", "0.55in", "8pt", "Normal", "#374151", "Left")
            TUnder(sb, "TxFcha", fecha, "0.52in", FmtIn(amtLeft + 0.58), "0.20in", FmtIn(cW - amtLeft - 0.58))

            ' ── Líneas de formulario ─────────────────────────────────────────
            Dim y As Double = hdr.heightUsed

            ' Recibí de: (propietario de la remedida)
            T(sb, "LbRec", "Recib" & Chr(237) & " de:", FmtIn(y), "0in", "0.22in", "1.05in", "9pt", "Bold", "#374151", "Left")
            TUnder(sb, "VlRec", Sanitize(propietario), FmtIn(y), "1.05in", "0.22in", FmtIn(cW - 1.05))
            y += 0.36

            ' La Cantidad de:
            T(sb, "LbCant", "La Cantidad de:", FmtIn(y), "0in", "0.22in", "1.38in", "9pt", "Bold", "#374151", "Left")
            TUnder(sb, "VlCant", Sanitize(montoLetras), FmtIn(y), "1.38in", "0.22in", FmtIn(cW - 1.38), "8pt")
            y += 0.36

            ' Por Concepto de:
            T(sb, "LbConc", "Por Concepto de:", FmtIn(y), "0in", "0.22in", "1.50in", "9pt", "Bold", "#374151", "Left")
            TUnder(sb, "VlCnc1", Sanitize(concepto), FmtIn(y), "1.50in", "0.22in", FmtIn(cW - 1.50))
            y += 0.34
            TUnder(sb, "VlCnc2", "", FmtIn(y), "0in", "0.22in", FmtIn(cW))
            y += 0.34

            Sep(sb, "S1", FmtIn(y), FmtIn(cW), "#CBD5E1", "1pt")
            y += 0.14

            ' ── Fila inferior: Firma | Recibo Interno | Tabla ───────────────
            Dim firmaY = y + 0.55
            Sep(sb, "SgFirma", FmtIn(firmaY), "2.60in", "#374151", "1pt")
            T(sb, "LbFirma", "FIRMA", FmtIn(firmaY + 0.06), "0.70in", "0.22in", "1.20in", "9pt", "Bold", "#374151", "Center")

            Dim riLeft = 2.75
            Dim riW = 1.70
            T(sb, "TxRiLbl", "Recibo Interno", FmtIn(y + 0.15), FmtIn(riLeft), "0.24in", FmtIn(riW), "9pt", "Bold", "#D97706", "Center")
            T(sb, "TxRiNum", "No. " & cobro.Id.ToString(), FmtIn(y + 0.42), FmtIn(riLeft), "0.28in", FmtIn(riW), "12pt", "Bold", "#D97706", "Center")

            Dim tL = 4.55
            Dim cLbl = 1.05
            Dim cVal = 1.10
            Dim rH = 0.22
            Dim tY = y

            TBg(sb, "ThLbl", "", FmtIn(tY), FmtIn(tL), FmtIn(rH), FmtIn(cLbl), "7pt", "Bold", "#E2E8F0", "#94A3B8", "Center", "#374151")
            TBg(sb, "ThV1", "Monto (L.)", FmtIn(tY), FmtIn(tL + cLbl), FmtIn(rH), FmtIn(cVal), "7pt", "Bold", "#E2E8F0", "#94A3B8", "Center", "#374151")
            tY += rH

            TBg(sb, "TrPtL", "Precio Total", FmtIn(tY), FmtIn(tL), FmtIn(rH), FmtIn(cLbl), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Left", "#374151")
            TBg(sb, "TrPtV", Sanitize(precioTotalStr), FmtIn(tY), FmtIn(tL + cLbl), FmtIn(rH), FmtIn(cVal), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Center", "#374151")
            tY += rH

            TBg(sb, "TrAbL", "Abono", FmtIn(tY), FmtIn(tL), FmtIn(rH), FmtIn(cLbl), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Left", "#374151")
            TBg(sb, "TrAbV", Sanitize(abonoStr), FmtIn(tY), FmtIn(tL + cLbl), FmtIn(rH), FmtIn(cVal), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Center", "#374151")
            tY += rH

            TBg(sb, "TrSdL", "Saldo", FmtIn(tY), FmtIn(tL), FmtIn(rH), FmtIn(cLbl), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Left", "#374151")
            TBg(sb, "TrSdV", Sanitize(saldoStr), FmtIn(tY), FmtIn(tL + cLbl), FmtIn(rH), FmtIn(cVal), "7pt", "Normal", "#FFFFFF", "#CBD5E1", "Center", "#374151")
            tY += rH

            Dim tableBottom = tY

            ' ── Pie ─────────────────────────────────────────────────────────
            Dim ftY = Math.Max(firmaY + 0.42, tableBottom + 0.10)
            Sep(sb, "SFt", FmtIn(ftY), FmtIn(cW), "#E2E8F0", "1pt")
            T(sb, "TxFt", "CONSTRUCTORA VEMAR S. DE R.L. DE C.V. - Documento generado electronicamente",
              FmtIn(ftY + 0.06), "0in", "0.18in", FmtIn(cW), "7pt", "Normal", "#94A3B8", "Center")

            Dim rcContentH As Double = ftY + 0.26
            Dim rcMaxBodyH As Double = pH - 0.50 * 2 - 0.02
            sb.Append($"</ReportItems><Height>{FmtIn(Math.Min(rcContentH, rcMaxBodyH))}</Height></Body>")
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

        Private Function NumerosALetras(n As Decimal) As String
            Dim entero = CLng(Math.Floor(n))
            If entero = 0 Then Return "CERO LEMPIRAS"
            Dim unidades() As String = {"", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE",
                                        "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE"}
            Dim decenas() As String = {"", "", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA"}
            Dim centenas() As String = {"", "CIEN", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS",
                                        "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS"}

            Dim Grupo = Function(num As Long) As String
                            Dim r = ""
                            Dim c = num \ 100 : Dim resto = num Mod 100
                            If c > 0 Then
                                r = If(c = 1 AndAlso resto > 0, "CIENTO", centenas(c))
                            End If
                            If resto > 0 Then
                                If r.Length > 0 Then r &= " "
                                If resto < 20 Then
                                    r &= unidades(resto)
                                Else
                                    r &= decenas(resto \ 10)
                                    If resto Mod 10 > 0 Then r &= " Y " & unidades(resto Mod 10)
                                End If
                            End If
                            Return r
                        End Function

            Dim resultado = ""
            Dim millones = entero \ 1000000
            Dim miles = (entero Mod 1000000) \ 1000
            Dim restoFinal = entero Mod 1000

            If millones > 0 Then resultado &= If(millones = 1, "UN MILLÓN", Grupo(millones) & " MILLONES") & " "
            If miles > 0 Then resultado &= If(miles = 1, "MIL", Grupo(miles) & " MIL") & " "
            If restoFinal > 0 Then resultado &= Grupo(restoFinal) & " "

            Return resultado.Trim() & " LEMPIRAS"
        End Function

        Private Function Sanitize(s As String) As String
            If String.IsNullOrEmpty(s) Then Return "N/D"
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
