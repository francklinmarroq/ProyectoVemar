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
            Dim clienteRtn = XmlEscOpt(p.Cliente?.Rtn)
            Dim clienteRep = XmlEscOpt(p.Cliente?.Representante)
            Dim clienteRepDni = XmlEscOpt(p.Cliente?.DniRepresentante)
            Dim clienteRepRtn = XmlEscOpt(p.Cliente?.RtnRepresentante)
            Dim clienteTel = XmlEscOpt(p.Cliente?.Telefono)
            Dim clienteDireccion = XmlEscOpt(p.Cliente?.Direccion)
            Dim clienteDniProp = XmlEscOpt(p.Cliente?.DniPropietario)
            Dim clienteTelRep = XmlEscOpt(p.Cliente?.TelefonoRepresentante)
            Dim clienteEmailRep = XmlEscOpt(p.Cliente?.EmailRepresentante)
            Dim clienteEmailCorp = XmlEscOpt(p.Cliente?.EmailCorporativo)
            Dim descripcion = XmlEscOpt(p.Descripcion)
            Dim valorProyecto = If(p.ValorProyecto > 0D, "L " & p.ValorProyecto.ToString("N2", CultureInfo.InvariantCulture), "")
            Dim proyNombre = XmlEsc(If(p.Nombre, "—"))
            Dim claveSure = XmlEsc(If(p.ClaveSure, "—"))
            Dim matricula = XmlEsc(If(p.Matricula, "—"))
            Dim ubicacion = XmlEscOpt(p.Ubicacion)
            Dim categoria = XmlEscOpt(p.CategoriaProyecto?.Nombre)
            Dim zonif = XmlEscOpt(p.Zonificacion?.Zonificacion)

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
            Dim hdr = ReportHeaderHelper.BuildHeader(logoB64, cW, "bp")
            sb.Append(hdr.xml)

            T(sb, "TxTit", "ACTA DE COMPROMISO DEL URBANIZADOR", FmtIn(hdr.heightUsed + 0.05), "0in", "0.25in", FmtIn(cW), "11pt", "Bold", "#1E40AF", "Left")
            T(sb, "TxGen", $"Fecha: {fechaGen}    |    Registro No. {p.Id}", FmtIn(hdr.heightUsed + 0.30), "0in", "0.18in", FmtIn(cW), "8pt", "Normal", "#64748B", "Left")

            ' ── Nombre del proyecto ──
            Dim nTop = hdr.heightUsed + 0.56
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
            ' Solo se muestran los campos que realmente tienen datos, para no dejar
            ' cuadros vacíos sin etiqueta ni valor cuando el dato opcional no existe.
            Dim gTop = idTop + 0.62 + 0.12
            Dim gRowH = 0.38

            Dim allFields As New List(Of (Label As String, Value As String)) From {
                ("Cliente / Urbanizador", clienteNombre),
                ("R.T.N. Cliente", clienteRtn),
                ("DNI Propietario", clienteDniProp),
                ("Teléfono Cliente", clienteTel),
                ("Dirección Cliente", clienteDireccion),
                ("Correo Corporativo", clienteEmailCorp),
                ("Representante Legal", clienteRep),
                ("DNI Representante", clienteRepDni),
                ("RTN Representante", clienteRepRtn),
                ("Teléfono Representante", clienteTelRep),
                ("Correo Representante", clienteEmailRep),
                ("Categoría", categoria),
                ("Zonificación", zonif),
                ("Área", area),
                ("Valor del Proyecto", valorProyecto),
                ("Ubicación", ubicacion)
            }
            Dim fields = allFields.Where(Function(f) Not String.IsNullOrWhiteSpace(f.Value)).ToList()

            ' La boleta debe caber SIEMPRE en una sola página. Se calcula el espacio
            ' libre y, si con 2 columnas las filas quedarían demasiado bajas, se pasa
            ' a 3 columnas antes de comprimir la altura de las filas.
            Dim maxBodyH As Double = pH - 0.50 * 2 - 0.02
            ' La descripción va en su propia fila a ancho completo (puede ser larga).
            Dim descH As Double = If(String.IsNullOrWhiteSpace(descripcion), 0.0, 0.62)
            Dim afterGridH As Double = descH + 0.18 + 1.05 + 0.30 + 1.7 + 0.26
            Dim avail As Double = maxBodyH - gTop - afterGridH

            Dim cols As Integer = 2
            Dim rowCount As Integer = CInt(Math.Ceiling(fields.Count / CDbl(cols)))
            Dim rowPitch As Double = gRowH + 0.06
            If rowCount > 0 AndAlso avail / rowCount < 0.36 Then
                cols = 3
                rowCount = CInt(Math.Ceiling(fields.Count / CDbl(cols)))
            End If
            If rowCount > 0 Then
                rowPitch = Math.Max(0.3, Math.Min(rowPitch, avail / rowCount))
                gRowH = rowPitch - 0.06
            End If

            Dim gapX As Double = 0.1
            Dim colW As Double = (cW - gapX * (cols - 1)) / cols

            Dim rowIdx As Integer = 0
            Dim fi As Integer = 0
            Do While fi < fields.Count
                Dim rowTop = FmtIn(gTop + rowPitch * rowIdx)
                Dim remaining = fields.Count - fi
                If remaining >= cols Then
                    For c = 0 To cols - 1
                        FieldCell(sb, "F" & (fi + c), fields(fi + c).Label, fields(fi + c).Value,
                                  rowTop, FmtIn((colW + gapX) * c), FmtIn(colW), FmtIn(gRowH))
                    Next
                    fi += cols
                Else
                    ' Última fila incompleta: se reparte el ancho entre los que quedan.
                    Dim lastW = (cW - gapX * (remaining - 1)) / remaining
                    For c = 0 To remaining - 1
                        FieldCell(sb, "F" & (fi + c), fields(fi + c).Label, fields(fi + c).Value,
                                  rowTop, FmtIn((lastW + gapX) * c), FmtIn(lastW), FmtIn(gRowH))
                    Next
                    fi += remaining
                End If
                rowIdx += 1
            Loop

            Dim gridBottom = gTop + rowPitch * rowIdx
            If descH > 0 Then
                FieldCell(sb, "FDesc", "Descripción", descripcion,
                          FmtIn(gridBottom), "0in", FmtIn(cW), FmtIn(descH - 0.06))
                gridBottom += descH
            End If

            ' ── Texto de compromiso ──
            Dim cmpTop = gridBottom + 0.18
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

            Dim bpContentH As Double = footTop + 0.26
            sb.Append($"</ReportItems><Height>{FmtIn(Math.Min(bpContentH, maxBodyH))}</Height></Body>")
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

        ' Dibuja un campo (etiqueta + valor) como UN solo cuadro de texto con dos
        ' párrafos, de modo que la etiqueta y el valor nunca puedan desalinearse.
        Private Sub FieldCell(sb As StringBuilder, name As String, label As String, value As String,
                              top As String, left As String, width As String, height As String)
            ' Altura fija (sin CanGrow): un valor largo no debe empujar el resto del
            ' contenido a una segunda página.
            sb.Append($"<Textbox Name=""Cl{name}""><CanGrow>false</CanGrow><CanShrink>false</CanShrink><Paragraphs>")
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
