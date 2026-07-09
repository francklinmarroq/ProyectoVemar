Imports System.Text
Imports System.Globalization

Namespace Vemar.WPF.Reports

    ''' Genera el XML del encabezado corporativo estándar para todos los PDFs.
    ''' Devuelve el bloque de ReportItems (sin wrapper) y la altura usada en pulgadas.
    Public Module ReportHeaderHelper

        ''' <param name="logoB64">Base64 del logo; String.Empty si no hay.</param>
        ''' <param name="cW">Ancho del contenido disponible en pulgadas.</param>
        ''' <param name="idPrefix">Prefijo único para nombres de elementos RDLC.</param>
        ''' <returns>Tupla con el XML del header y los pulgadas usadas (para que el contenido continúe debajo).</returns>
        Public Function BuildHeader(logoB64 As String, cW As Double, idPrefix As String) As (xml As String, heightUsed As Double)
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)
            Dim sb As New StringBuilder()
            Dim IC = CultureInfo.InvariantCulture
            Dim F = Function(d As Double) d.ToString("F2", IC) & "in"

            Const hdrH As Double = 1.02    ' altura del área gris
            Const sepH As Double = 0.04    ' grosor línea azul
            Const titleH As Double = 0.18  ' altura fila título
            Dim totalH As Double = hdrH + sepH + 0.06 + titleH

            ' Fondo gris
            sb.Append($"<Rectangle Name=""{idPrefix}BgHdr"">")
            sb.Append($"<Top>0in</Top><Left>0in</Left><Height>{F(hdrH)}</Height><Width>{F(cW)}</Width>")
            sb.Append("<Style><BackgroundColor>#F8FAFC</BackgroundColor></Style>")
            sb.Append("</Rectangle>")

            ' Logo
            If hasLogo Then
                sb.Append($"<Image Name=""{idPrefix}Logo"">")
                sb.Append("<Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append($"<Top>0.08in</Top><Left>0in</Left><Height>0.85in</Height><Width>0.85in</Width>")
                sb.Append("<Style/>")
                sb.Append("</Image>")
            End If

            Dim lx = If(hasLogo, 0.95, 0.0)
            Dim txtW = cW - lx - 0.95
            Dim lxF = F(lx)
            Dim twF = F(txtW)
            Dim pgL = F(cW - 0.95)

            ' "CONSTRUCTORA VEMAR…" — gris pequeño centrado
            Tb(sb, $"{idPrefix}Sub", "CONSTRUCTORA VEMAR S. de R.L. de C.V.",
               "0.06in", lxF, "0.17in", twF, "7pt", "Normal", "#94A3B8", "Center", False)

            ' "VEMAR" — grande azul centrado
            Tb(sb, $"{idPrefix}Brand", "VEMAR",
               "0.22in", lxF, "0.36in", twF, "22pt", "Bold", "#1E3A8A", "Center", False)

            ' Slogan — itálica gris centrado
            Tb(sb, $"{idPrefix}Slogan", "Consultoría  ·  Ambiente  ·  Obra Civil",
               "0.59in", lxF, "0.16in", twF, "8pt", "Normal", "#64748B", "Center", True)

            ' Email | RTN — gris claro centrado
            Tb(sb, $"{idPrefix}Ctc", "constructora.vemar@yahoo.com  |  RTN: 03019012468535",
               "0.77in", lxF, "0.14in", twF, "7pt", "Normal", "#94A3B8", "Center", False)

            ' Fecha generación — arriba derecha
            Tb(sb, $"{idPrefix}Fecha", "Generado: " & DateTime.Now.ToString("dd/MM/yyyy"),
               "0.06in", pgL, "0.16in", "0.95in", "7pt", "Normal", "#94A3B8", "Right", False)

            ' Línea azul separadora
            sb.Append($"<Rectangle Name=""{idPrefix}Sep"">")
            sb.Append($"<Top>{F(hdrH)}</Top><Left>0in</Left><Height>{F(sepH)}</Height><Width>{F(cW)}</Width>")
            sb.Append("<Style><BackgroundColor>#1E3A8A</BackgroundColor></Style>")
            sb.Append("</Rectangle>")

            Return (sb.ToString(), totalH)
        End Function

        Private Sub Tb(sb As StringBuilder, name As String, text As String,
                       top As String, left As String, height As String, width As String,
                       fontSize As String, weight As String, color As String,
                       align As String, italic As Boolean)
            Dim safe = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
            sb.Append($"<Textbox Name=""{name}"">")
            sb.Append("<CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append($"<Style><TextAlign>{align}</TextAlign></Style>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append($"<Value>{safe}</Value>")
            sb.Append("<Style>")
            sb.Append($"<FontSize>{fontSize}</FontSize>")
            If weight = "Bold" Then sb.Append("<FontWeight>Bold</FontWeight>")
            If italic Then sb.Append("<FontStyle>Italic</FontStyle>")
            sb.Append($"<Color>{color}</Color>")
            sb.Append("</Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("</Textbox>")
        End Sub

    End Module

End Namespace
