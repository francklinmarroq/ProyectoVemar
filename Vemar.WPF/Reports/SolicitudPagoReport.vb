Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports Microsoft.Reporting.NETCore
Imports Vemar.Domain

Namespace Vemar.WPF.Reports

    Public Class SolicitudPagoReport

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

        Public Async Function GeneratePdfAsync(proyecto As Proyecto, valorTotal As Decimal, cobrosAnteriores As List(Of CobroProyecto), montoSolicitud As Decimal) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rdlcXml = BuildRdlcXml(proyecto, valorTotal, cobrosAnteriores, montoSolicitud)
                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using

                    Dim pdfBytes = report.Render("PDF")
                    Dim nombre = If(String.IsNullOrWhiteSpace(proyecto.Nombre), "proyecto", proyecto.Nombre)
                    Dim nSolicitud = cobrosAnteriores.Count + 1
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Solicitud de Pago", $"SolicitudPago_{nombre}_N{nSolicitud}_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
                Catch ex As Exception
                    Dim msg = ex.Message
                    Dim inner = ex.InnerException
                    Do While inner IsNot Nothing
                        msg &= " → " & inner.Message
                        inner = inner.InnerException
                    Loop
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar solicitud: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        Private Function BuildRdlcXml(proyecto As Proyecto, valorContrato As Decimal, pagos As List(Of CobroProyecto), montoSolicitud As Decimal) As String
            Const pW As Double = 8.5
            Const pH As Double = 11.0
            Const mg As Double = 0.6
            Const cW As Double = 7.3

            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)

            Dim cliente = proyecto.Cliente
            Dim nSolicitud = pagos.Count + 1

            ' Calcular saldo actual
            Dim totalPagado = pagos.Sum(Function(p) p.Monto)
            Dim saldoActual = valorContrato - totalPagado

            Dim clienteNombre = If(cliente?.Nombre, "").ToUpper()
            Dim fechaDoc = DateTime.Now.ToString("dd 'DE' MMMM 'DEL' yyyy", New CultureInfo("es-ES")).ToUpper()
            Dim proyectoNombre = If(proyecto.Nombre, "").ToUpper()
            Dim proyectoDesc = If(proyecto.Ubicacion, "")

            Dim IC = CultureInfo.InvariantCulture
            Dim Fmt = Function(v As Decimal) "L " & v.ToString("N2", IC)
            Dim FmtFecha = Function(d As DateTime) d.ToString("d/MM/yyyy")

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

            Dim y As Double = 0.0

            ' ── Helpers ──────────────────────────────────────────────────────────
            Dim nextId As Integer = 1
            Dim Id = Function() As String
                         Dim r = "i" & nextId.ToString()
                         nextId += 1
                         Return r
                     End Function

            ' Textbox helper
            Dim T = Sub(left As Double, top As Double, w As Double, h As Double,
                        text As String, fontSize As Integer,
                        bold As Boolean, align As String, color As String,
                        bgColor As String, italic As Boolean,
                        border As String, borderColor As String)
                        Dim safe = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
                        sb.Append($"<Textbox Name=""{Id()}"">")
                        sb.Append($"<Left>{left}in</Left><Top>{top}in</Top><Width>{w}in</Width><Height>{h}in</Height>")
                        sb.Append("<Style>")
                        If bgColor <> "Transparent" Then sb.Append($"<BackgroundColor>{bgColor}</BackgroundColor>")
                        sb.Append("<VerticalAlign>Middle</VerticalAlign>")
                        sb.Append("<PaddingLeft>4pt</PaddingLeft><PaddingRight>4pt</PaddingRight>")
                        If border <> "None" Then
                            sb.Append($"<Border><Color>{borderColor}</Color><Style>{border}</Style></Border>")
                        End If
                        sb.Append("</Style>")
                        sb.Append("<Paragraphs><Paragraph>")
                        sb.Append($"<Style><TextAlign>{align}</TextAlign></Style>")
                        sb.Append("<TextRuns><TextRun>")
                        sb.Append($"<Value>{safe}</Value>")
                        sb.Append("<Style>")
                        sb.Append($"<FontSize>{fontSize}pt</FontSize>")
                        If bold Then sb.Append("<FontWeight>Bold</FontWeight>")
                        If italic Then sb.Append("<FontStyle>Italic</FontStyle>")
                        sb.Append($"<Color>{color}</Color>")
                        sb.Append("</Style>")
                        sb.Append("</TextRun></TextRuns>")
                        sb.Append("</Paragraph></Paragraphs>")
                        sb.Append("</Textbox>")
                    End Sub

            ' Rectangle (box without text)
            Dim Rect = Sub(left As Double, top As Double, w As Double, h As Double,
                           bgColor As String,
                           borderStyle As String,
                           borderColor As String,
                           borderWidth As String)
                           sb.Append($"<Rectangle Name=""{Id()}"">")
                           sb.Append($"<Left>{left}in</Left><Top>{top}in</Top><Width>{w}in</Width><Height>{h}in</Height>")
                           sb.Append("<Style>")
                           If bgColor <> "Transparent" Then sb.Append($"<BackgroundColor>{bgColor}</BackgroundColor>")
                           If borderStyle <> "None" AndAlso borderColor <> "Transparent" Then
                               sb.Append($"<Border><Color>{borderColor}</Color><Style>{borderStyle}</Style><Width>{borderWidth}</Width></Border>")
                           End If
                           sb.Append("</Style>")
                           sb.Append("</Rectangle>")
                       End Sub

            ' ── FONDO HEADER ──────────────────────────────────────────────────────
            Dim hdrH2 = 1.0
            Rect(mg, y, cW - mg, hdrH2, "#F8FAFC", "None", "#000000", "0pt")

            ' ── LOGO (izquierda) ──────────────────────────────────────────────────
            If hasLogo Then
                sb.Append($"<Image Name=""{Id()}"">")
                sb.Append($"<Left>{mg + 0.05}in</Left><Top>{y + 0.1}in</Top><Width>1.1in</Width><Height>0.8in</Height>")
                sb.Append("<Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("</Image>")
            End If

            ' ── MEMBRETE (centro) ─────────────────────────────────────────────────
            Dim mLeft = mg + 1.25
            Dim mW = cW - mg - 1.25 - 0.9
            T(mLeft, y + 0.08, mW, 0.18, "CONSTRUCTORA VEMAR S. de R.L. de C.V.", 7, False, "Center", "#94A3B8", "Transparent", False, "None", "#000000")
            T(mLeft, y + 0.26, mW, 0.38, "VEMAR", 22, True, "Center", "#1E3A8A", "Transparent", False, "None", "#000000")
            T(mLeft, y + 0.62, mW, 0.16, "Consultoría  ·  Ambiente  ·  Obra Civil", 8, False, "Center", "#64748B", "Transparent", True, "None", "#000000")
            T(mLeft, y + 0.80, mW, 0.14, "constructora.vemar@yahoo.com  |  RTN: 03019012468535", 7, False, "Center", "#94A3B8", "Transparent", False, "None", "#000000")

            ' ── NÚMERO DE PÁGINA (derecha) ──────────────────────────────────────
            T(mg + cW - mg - 0.9, y + 0.08, 0.9, 0.16, "Pág. 1 / 1", 7, False, "Right", "#94A3B8", "Transparent", False, "None", "#000000")

            y += hdrH2

            ' ── LÍNEA AZUL DIVISORIA ─────────────────────────────────────────────
            Rect(mg, y, cW - mg, 0.04, "#1E3A8A", "None", "#000000", "0pt")
            y += 0.1

            ' ── TÍTULO SOLICITUD DE PAGO ─────────────────────────────────────────
            y += 0.15
            T(mg, y, cW, 0.28, "SOLICITUD DE PAGO", 13, True, "Center", "#000000", "Transparent", False, "None", "#000000")
            y += 0.38

            ' ── CAJA AMARILLA CON DATOS DEL CLIENTE/PROYECTO ─────────────────────
            Dim boxH = 1.1
            Rect(mg, y, cW, boxH, "#FFFDE7", "Solid", "#C8A000", "1pt")

            Dim lx = mg + 0.08
            Dim bLine = 0.18
            T(lx, y + 0.04, 1.2, bLine, "CLIENTE:", 9, True, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx + 1.2, y + 0.04, cW - 1.28, bLine, clienteNombre, 9, False, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx, y + 0.04 + bLine, 1.2, bLine, "FECHA:", 9, True, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx + 1.2, y + 0.04 + bLine, cW - 1.28, bLine, fechaDoc, 9, False, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx, y + 0.04 + bLine * 2, 1.8, bLine, "NOMBRE DEL PROYECTO:", 9, True, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx + 1.8, y + 0.04 + bLine * 2, cW - 1.88, bLine, proyectoNombre, 9, False, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx, y + 0.04 + bLine * 3, 2.1, bLine, "DESCRIPCIÓN DEL PROYECTO:", 9, True, "Left", "#000000", "Transparent", False, "None", "#000000")
            T(lx + 2.1, y + 0.04 + bLine * 3, cW - 2.18, bLine, proyectoDesc, 9, False, "Left", "#000000", "Transparent", False, "None", "#000000")

            y += boxH + 0.18

            ' ── TABLA DE PAGOS ────────────────────────────────────────────────────
            ' Columnas suman 6.7in (= cW - mg) para no desbordar el body
            ' ITEM=0.4, DESC=2.1, FORMA=1.2, FECHA=0.95, MONTO=1.05, SALDO=1.0
            Dim colX() As Double = {mg, mg + 0.4, mg + 2.5, mg + 3.7, mg + 4.65, mg + 5.7}
            Dim colW() As Double = {0.4, 2.1, 1.2, 0.95, 1.05, 1.0}
            Dim hdrH = 0.28

            ' Encabezado tabla
            Dim hdrBg = "#1E3A8A"
            T(colX(0), y, colW(0), hdrH, "ITEM", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            T(colX(1), y, colW(1), hdrH, "DESCRIPCIÓN DE PAGOS", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            T(colX(2), y, colW(2), hdrH, "FORMA DE PAGO", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            T(colX(3), y, colW(3), hdrH, "FECHA", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            T(colX(4), y, colW(4), hdrH, "MONTO", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            T(colX(5), y, colW(5), hdrH, "SALDO", 8, True, "Center", "White", hdrBg, False, "Solid", "#1E3A8A")
            y += hdrH

            ' Fila 1: Monto del proyecto (total contrato)
            Dim rowH = 0.22
            Dim altBg = "#F8FAFC"
            T(colX(0), y, colW(0), rowH, "1", 9, False, "Center", "#000000", "White", False, "Solid", "#D1D5DB")
            T(colX(1), y, colW(1), rowH, "Monto del proyecto", 9, False, "Left", "#000000", "White", False, "Solid", "#D1D5DB")
            T(colX(2), y, colW(2), rowH, "", 9, False, "Center", "#000000", "White", False, "Solid", "#D1D5DB")
            T(colX(3), y, colW(3), rowH, "", 9, False, "Center", "#000000", "White", False, "Solid", "#D1D5DB")
            T(colX(4), y, colW(4), rowH, "", 9, False, "Right", "#000000", "White", False, "Solid", "#D1D5DB")
            T(colX(5), y, colW(5), rowH, Fmt(valorContrato), 9, True, "Right", "#000000", "White", False, "Solid", "#D1D5DB")
            y += rowH

            ' Filas de cobros anteriores al cliente
            Dim saldoCorr = valorContrato
            For i = 0 To pagos.Count - 1
                Dim p = pagos(i)
                saldoCorr -= p.Monto
                Dim rowBg = If(i Mod 2 = 0, altBg, "White")
                Dim nPago = i + 1
                Dim fechaStr = If(p.Fecha = Date.MinValue, "", FmtFecha(p.Fecha))
                Dim desc = If(String.IsNullOrWhiteSpace(p.Descripcion), $"COBRO N°{nPago}", p.Descripcion.ToUpper())
                Dim forma = If(String.IsNullOrWhiteSpace(p.FormaPago), "", p.FormaPago)
                T(colX(0), y, colW(0), rowH, (nPago + 1).ToString(), 9, False, "Center", "#000000", rowBg, False, "Solid", "#D1D5DB")
                T(colX(1), y, colW(1), rowH, desc, 9, False, "Left", "#000000", rowBg, False, "Solid", "#D1D5DB")
                T(colX(2), y, colW(2), rowH, forma, 9, False, "Center", "#000000", rowBg, False, "Solid", "#D1D5DB")
                T(colX(3), y, colW(3), rowH, fechaStr, 9, False, "Center", "#000000", rowBg, False, "Solid", "#D1D5DB")
                T(colX(4), y, colW(4), rowH, Fmt(p.Monto), 9, False, "Right", "#000000", rowBg, False, "Solid", "#D1D5DB")
                T(colX(5), y, colW(5), rowH, Fmt(saldoCorr), 9, False, "Right", "#000000", rowBg, False, "Solid", "#D1D5DB")
                y += rowH
            Next

            ' Fila solicitud actual (resaltada en amarillo)
            Dim nSol = pagos.Count + 2
            Dim solicitudDesc = $"SOLICITUD DE PAGO N°{nSolicitud}"
            Dim saldoTrasEsta = saldoCorr - montoSolicitud
            Rect(colX(0), y, cW - mg, rowH, "#FFFDE7", "Solid", "#C8A000", "1pt")
            T(colX(0), y, colW(0), rowH, nSol.ToString(), 9, True, "Center", "#000000", "Transparent", False, "Solid", "#C8A000")
            T(colX(1), y, colW(1), rowH, solicitudDesc, 9, True, "Left", "#000000", "Transparent", False, "Solid", "#C8A000")
            T(colX(2), y, colW(2), rowH, "Por cobrar", 9, True, "Center", "#555555", "Transparent", False, "Solid", "#C8A000")
            T(colX(3), y, colW(3), rowH, DateTime.Now.ToString("d/MM/yyyy"), 9, True, "Center", "#555555", "Transparent", False, "Solid", "#C8A000")
            T(colX(4), y, colW(4), rowH, Fmt(montoSolicitud), 9, True, "Right", "#000000", "Transparent", False, "Solid", "#C8A000")
            T(colX(5), y, colW(5), rowH, Fmt(saldoTrasEsta), 9, True, "Right", "#000000", "Transparent", False, "Solid", "#C8A000")
            y += rowH + 0.18

            ' ── TOTAL A CANCELAR ─────────────────────────────────────────────────
            Dim saldoEntero = Math.Floor(montoSolicitud)
            Dim totalTexto = $"TOTAL, A CANCELAR: {Fmt(montoSolicitud)} ({NumerosALetras(saldoEntero)} con 00/100)"
            T(mg, y, cW, 0.22, totalTexto, 9, True, "Center", "#000000", "Transparent", False, "None", "#000000")
            y += 0.35

            ' ── FIRMA ────────────────────────────────────────────────────────────
            Dim sigX = mg + cW / 2 - 1.1
            Rect(sigX, y, 2.2, 0.6, "Transparent", "None", "Transparent", "0pt")
            ' Línea de firma
            T(sigX, y + 0.58, 2.2, 0.02, "", 6, False, "Center", "#000000", "#000000", False, "None", "#000000")
            y += 0.65
            T(sigX, y, 2.2, 0.18, "Ing. Josué Reynaldo Vega", 9, True, "Center", "#000000", "Transparent", False, "None", "#000000")
            T(sigX, y + 0.18, 2.2, 0.16, "Gerente General", 9, False, "Center", "#000000", "Transparent", False, "None", "#000000")
            T(sigX, y + 0.34, 2.2, 0.16, "Constructora VEMAR S. de R. L. de C. V.", 8, False, "Center", "#555555", "Transparent", False, "None", "#000000")
            y += 0.6

            ' ── CC ARCHIVO ───────────────────────────────────────────────────────
            y += 0.25
            T(mg, y, 2.0, 0.16, "CC. Archivo.", 8, False, "Left", "#666666", "Transparent", False, "None", "#000000")

            sb.Append("</ReportItems>")
            sb.Append($"<Height>{pH - 0.5}in</Height>")
            sb.Append("</Body>")
            sb.Append($"<Width>{pW}in</Width>")
            sb.Append("<Page>")
            sb.Append($"<PageHeight>{pH}in</PageHeight>")
            sb.Append($"<PageWidth>{pW}in</PageWidth>")
            sb.Append("<TopMargin>0.1in</TopMargin>")
            sb.Append("<BottomMargin>0.1in</BottomMargin>")
            sb.Append("<LeftMargin>0in</LeftMargin>")
            sb.Append("<RightMargin>0in</RightMargin>")
            sb.Append("</Page>")
            sb.Append("</Report>")
            Return sb.ToString()
        End Function

        Private Shared Function NumerosALetras(n As Decimal) As String
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

    End Class

End Namespace
