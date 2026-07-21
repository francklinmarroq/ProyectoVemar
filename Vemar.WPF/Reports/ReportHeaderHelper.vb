Imports System.Text
Imports System.Globalization
Imports System.IO
Imports System.Reflection

Namespace Vemar.WPF.Reports

    Public Module ReportHeaderHelper

        Private _bannerB64 As String = Nothing

        Public Function GetBannerBase64() As String
            If _bannerB64 IsNot Nothing Then Return _bannerB64
            Try
                Dim asm = Assembly.GetExecutingAssembly()
                Using stream = asm.GetManifestResourceStream("Vemar.WPF.PortadaVemar.jpeg")
                    If stream Is Nothing Then
                        _bannerB64 = String.Empty
                        Return _bannerB64
                    End If
                    Dim bytes(CInt(stream.Length) - 1) As Byte
                    stream.Read(bytes, 0, bytes.Length)
                    _bannerB64 = Convert.ToBase64String(bytes)
                End Using
            Catch
                _bannerB64 = String.Empty
            End Try
            Return _bannerB64
        End Function

        ''' <param name="logoB64">Ya no se usa; se mantiene por compatibilidad de firma.</param>
        ''' <param name="cW">Ancho del contenido disponible en pulgadas.</param>
        ''' <param name="idPrefix">Prefijo único para nombres de elementos RDLC.</param>
        Public Function BuildHeader(logoB64 As String, cW As Double, idPrefix As String) As (xml As String, heightUsed As Double)
            Dim IC = CultureInfo.InvariantCulture
            Dim F = Function(d As Double) d.ToString("F2", IC) & "in"
            Dim sb As New StringBuilder()

            Dim bannerB64 = GetBannerBase64()
            Const bannerH As Double = 2.05   ' altura del banner en pulgadas
            Const sepH As Double = 0.04      ' línea separadora azul
            Const gap As Double = 0.08       ' espacio antes del contenido

            If Not String.IsNullOrEmpty(bannerB64) Then
                sb.Append($"<Image Name=""{idPrefix}Banner"">")
                sb.Append("<Source>Embedded</Source><Value>VemarBanner</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                Dim bannerW As Double = Math.Min(6.56, cW)
                Dim bannerLeft As Double = Math.Max(0, (cW - bannerW) / 2.0)
                sb.Append($"<Top>0in</Top><Left>{F(bannerLeft)}</Left><Height>{F(bannerH)}</Height><Width>{F(bannerW)}</Width>")
                sb.Append("<Style/>")
                sb.Append("</Image>")
            End If

            ' Línea azul separadora debajo del banner
            sb.Append($"<Rectangle Name=""{idPrefix}Sep"">")
            sb.Append($"<Top>{F(bannerH)}</Top><Left>0in</Left><Height>{F(sepH)}</Height><Width>{F(cW)}</Width>")
            sb.Append("<Style><BackgroundColor>#1E3A8A</BackgroundColor></Style>")
            sb.Append("</Rectangle>")

            Dim totalH As Double = bannerH + sepH + gap  ' 1.60 + 0.04 + 0.08 = 1.72
            Return (sb.ToString(), totalH)
        End Function

    End Module

End Namespace
