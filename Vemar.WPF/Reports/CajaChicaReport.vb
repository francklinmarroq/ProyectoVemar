Imports System.Data
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Windows
Imports ClosedXML.Excel
Imports Microsoft.Reporting.NETCore
Imports Vemar.Domain

Namespace Vemar.WPF.Reports

    Public Class CajaChicaReport

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

        ' ─── Fila del DataTable ───────────────────────────────────────────────
        Private Class CajaRow
            Public Property Tipo As String
            Public Property Fecha As String
            Public Property Concepto As String
            Public Property Factura As String
            Public Property Vinculo As String
            Public Property Entrada As String
            Public Property Salida As String
        End Class

        ' ─── Excel ────────────────────────────────────────────────────────────
        Public Async Function GenerateExcelAsync(items As List(Of CajaChica),
                                                  fechaDesde As String,
                                                  fechaHasta As String,
                                                  Optional allItems As List(Of CajaChica) = Nothing) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"EstadoCajaChica_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx")

                    Using wb As New XLWorkbook()
                        Dim ws = wb.Worksheets.Add("Caja Chica")
                        Dim r As Integer = 1
                        Const cols = 7

                        ' Encabezado empresa
                        ws.Cell(r, 1).Value = "CONSTRUCTORA VEMAR S. de R.L. de C.V."
                        With ws.Cell(r, 1).Style.Font : .Bold = True : .FontSize = 14 : .FontColor = XLColor.FromHtml("#1E3A8A") : End With
                        ws.Range(r, 1, r, cols).Merge() : r += 1

                        ws.Cell(r, 1).Value = "ESTADO DE CAJA CHICA"
                        With ws.Cell(r, 1).Style.Font : .Bold = True : .FontSize = 12 : .FontColor = XLColor.FromHtml("#1E40AF") : End With
                        ws.Range(r, 1, r, cols).Merge() : r += 1

                        Dim periodo = If(String.IsNullOrWhiteSpace(fechaDesde) AndAlso String.IsNullOrWhiteSpace(fechaHasta),
                                         "Todos los registros",
                                         $"Período: {fechaDesde} — {fechaHasta}")
                        ws.Cell(r, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}     {periodo}"
                        With ws.Cell(r, 1).Style.Font : .Italic = True : .FontColor = XLColor.FromHtml("#64748B") : .FontSize = 10 : End With
                        ws.Range(r, 1, r, cols).Merge() : r += 1

                        ' Línea separadora
                        Dim sep = ws.Range(r, 1, r, cols)
                        sep.Merge()
                        sep.Style.Border.BottomBorder = XLBorderStyleValues.Medium
                        sep.Style.Border.BottomBorderColor = XLColor.FromHtml("#1E40AF")
                        r += 1

                        ' ── Estado Actual de Caja Chica (totales globales) ──
                        Dim all = If(allItems, items)
                        Dim allEnt = all.Where(Function(i) i.TipoOperacion = "Entrada").Sum(Function(i) i.Monto)
                        Dim allSal = all.Where(Function(i) i.TipoOperacion = "Salida").Sum(Function(i) i.Monto)
                        Dim allSaldo = allEnt - allSal
                        Dim allSaldoColor = If(allSaldo >= 0, XLColor.FromHtml("#16A34A"), XLColor.FromHtml("#DC2626"))

                        ws.Cell(r, 1).Value = "ESTADO ACTUAL DE CAJA CHICA"
                        With ws.Cell(r, 1).Style.Font : .Bold = True : .FontSize = 10 : .FontColor = XLColor.FromHtml("#1E40AF") : End With
                        ws.Range(r, 1, r, cols).Merge()
                        ws.Range(r, 1, r, cols).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF")
                        r += 1

                        ' Fila de cabeceras estado actual
                        Dim hdrsEA = {"Total Entradas (L)", "Total Salidas (L)", "Saldo Actual (L)", "No. Movimientos", "", "", ""}
                        For i = 0 To cols - 1
                            ws.Cell(r, i + 1).Value = hdrsEA(i)
                            With ws.Cell(r, i + 1).Style.Font : .Bold = True : .FontSize = 9 : .FontColor = XLColor.FromHtml("#374151") : End With
                            ws.Cell(r, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9")
                        Next
                        r += 1

                        ws.Cell(r, 1).Value = allEnt.ToString("N2")
                        ws.Cell(r, 1).Style.Font.FontColor = XLColor.FromHtml("#16A34A")
                        ws.Cell(r, 1).Style.Font.Bold = True
                        ws.Cell(r, 2).Value = allSal.ToString("N2")
                        ws.Cell(r, 2).Style.Font.FontColor = XLColor.FromHtml("#DC2626")
                        ws.Cell(r, 2).Style.Font.Bold = True
                        ws.Cell(r, 3).Value = allSaldo.ToString("N2") & " L"
                        ws.Cell(r, 3).Style.Font.FontColor = allSaldoColor
                        ws.Cell(r, 3).Style.Font.Bold = True
                        ws.Cell(r, 3).Style.Font.FontSize = 11
                        ws.Cell(r, 4).Value = all.Count.ToString()
                        r += 1

                        ' Separador entre estado actual y tabla del período
                        ws.Range(r, 1, r, cols).Merge()
                        ws.Range(r, 1, r, cols).Style.Border.BottomBorder = XLBorderStyleValues.Thin
                        ws.Range(r, 1, r, cols).Style.Border.BottomBorderColor = XLColor.FromHtml("#CBD5E1")
                        r += 1

                        ' Encabezados
                        Dim headers = {"FECHA", "CONCEPTO", "N° FACTURA", "VÍNCULO", "TIPO", "ENTRADA (L)", "SALIDA (L)"}
                        For i = 0 To 6
                            ws.Cell(r, i + 1).Value = headers(i)
                            With ws.Cell(r, i + 1).Style
                                .Font.Bold = True
                                .Fill.BackgroundColor = XLColor.FromHtml("#1E40AF")
                                .Font.FontColor = XLColor.White
                                .Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                            End With
                        Next
                        r += 1

                        Dim totalEntradas As Decimal = 0
                        Dim totalSalidas As Decimal = 0

                        For Each item In items
                            Dim vinculo = GetVinculo(item)
                            Dim esEntrada = item.TipoOperacion = "Entrada"
                            If esEntrada Then totalEntradas += item.Monto Else totalSalidas += item.Monto

                            ws.Cell(r, 1).Value = item.Fecha.ToString("dd/MM/yyyy")
                            ws.Cell(r, 2).Value = item.Concepto
                            ws.Cell(r, 3).Value = If(item.NumeroFactura, "")
                            ws.Cell(r, 4).Value = vinculo
                            ws.Cell(r, 5).Value = item.TipoOperacion
                            ws.Cell(r, 5).Style.Font.FontColor = If(esEntrada, XLColor.FromHtml("#16A34A"), XLColor.FromHtml("#DC2626"))
                            If esEntrada Then
                                ws.Cell(r, 6).Value = item.Monto.ToString("N2")
                                ws.Cell(r, 6).Style.Font.FontColor = XLColor.FromHtml("#16A34A")
                                ws.Cell(r, 7).Value = ""
                            Else
                                ws.Cell(r, 6).Value = ""
                                ws.Cell(r, 7).Value = item.Monto.ToString("N2")
                                ws.Cell(r, 7).Style.Font.FontColor = XLColor.FromHtml("#DC2626")
                            End If
                            If r Mod 2 = 0 Then ws.Range(r, 1, r, cols).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC")
                            r += 1
                        Next

                        ' Totales
                        r += 1
                        Dim totRange = ws.Range(r, 1, r, cols)
                        totRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF")
                        totRange.Style.Border.TopBorder = XLBorderStyleValues.Double
                        totRange.Style.Border.TopBorderColor = XLColor.FromHtml("#1E40AF")

                        ws.Cell(r, 1).Value = "TOTALES"
                        ws.Range(r, 1, r, 5).Merge()
                        With ws.Cell(r, 1).Style.Font : .Bold = True : .FontSize = 11 : .FontColor = XLColor.FromHtml("#1E40AF") : End With

                        ws.Cell(r, 6).Value = totalEntradas.ToString("N2")
                        With ws.Cell(r, 6).Style.Font : .Bold = True : .FontColor = XLColor.FromHtml("#16A34A") : End With

                        ws.Cell(r, 7).Value = totalSalidas.ToString("N2")
                        With ws.Cell(r, 7).Style.Font : .Bold = True : .FontColor = XLColor.FromHtml("#DC2626") : End With
                        r += 1

                        ' Saldo
                        Dim saldo = totalEntradas - totalSalidas
                        Dim saldoColor = If(saldo >= 0, XLColor.FromHtml("#16A34A"), XLColor.FromHtml("#DC2626"))
                        Dim saldoRange = ws.Range(r, 1, r, cols)
                        saldoRange.Style.Fill.BackgroundColor = If(saldo >= 0, XLColor.FromHtml("#F0FDF4"), XLColor.FromHtml("#FFF1F2"))

                        ws.Cell(r, 1).Value = "SALDO  (Entradas - Salidas):"
                        ws.Range(r, 1, r, 5).Merge()
                        With ws.Cell(r, 1).Style.Font : .Bold = True : .FontSize = 12 : .FontColor = saldoColor : End With

                        ws.Cell(r, 6).Value = saldo.ToString("N2") & " L"
                        ws.Range(r, 6, r, cols).Merge()
                        With ws.Cell(r, 6).Style.Font : .Bold = True : .FontSize = 12 : .FontColor = saldoColor : End With
                        ws.Cell(r, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                        ws.Columns().AdjustToContents()
                        wb.SaveAs(filePath)
                    End Using

                    Process.Start(New ProcessStartInfo With {.FileName = filePath, .UseShellExecute = True})
                    Return filePath
                Catch ex As Exception
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar Excel: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        ' ─── PDF ──────────────────────────────────────────────────────────────
        Public Async Function GeneratePdfAsync(items As List(Of CajaChica),
                                                fechaDesde As String,
                                                fechaHasta As String,
                                                Optional allItems As List(Of CajaChica) = Nothing) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rows = BuildRows(items)
                    Dim dt = BuildDataTable(rows)
                    Dim rdlcXml = BuildRdlcXml(items, fechaDesde, fechaHasta, allItems)

                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using
                    report.DataSources.Add(New ReportDataSource("DataSet1", dt))

                    Dim pdfBytes = report.Render("PDF")
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Estado de Caja Chica", $"EstadoCajaChica_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
                Catch ex As Exception
                    Dim msg = BuildMsg(ex)
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar PDF: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        ' ─── Helpers ──────────────────────────────────────────────────────────
        Private Shared Function GetVinculo(item As CajaChica) As String
            If item.Remedida IsNot Nothing Then
                Dim clave = If(String.IsNullOrWhiteSpace(item.Remedida.ClaveSure), "#" & item.Remedida.Id.ToString(), item.Remedida.ClaveSure)
                Return "Remedida: " & clave
            ElseIf item.Proyecto IsNot Nothing Then
                Return "Proyecto: " & If(item.Proyecto.Nombre, "#" & item.Proyecto.Id.ToString())
            End If
            Return "—"
        End Function

        Private Function BuildRows(items As List(Of CajaChica)) As List(Of CajaRow)
            Dim rows As New List(Of CajaRow)()
            Dim totalEntradas As Decimal = 0
            Dim totalSalidas As Decimal = 0

            For Each item In items
                Dim esEntrada = item.TipoOperacion = "Entrada"
                If esEntrada Then totalEntradas += item.Monto Else totalSalidas += item.Monto

                rows.Add(New CajaRow With {
                    .Tipo = item.TipoOperacion,
                    .Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                    .Concepto = item.Concepto,
                    .Factura = If(item.NumeroFactura, ""),
                    .Vinculo = GetVinculo(item),
                    .Entrada = If(esEntrada, item.Monto.ToString("N2"), ""),
                    .Salida = If(Not esEntrada, item.Monto.ToString("N2"), "")
                })
            Next

            ' Fila totales
            rows.Add(New CajaRow With {
                .Tipo = "TOTAL",
                .Fecha = "",
                .Concepto = "TOTALES",
                .Factura = "",
                .Vinculo = "",
                .Entrada = totalEntradas.ToString("N2"),
                .Salida = totalSalidas.ToString("N2")
            })

            ' Fila saldo
            Dim saldo = totalEntradas - totalSalidas
            rows.Add(New CajaRow With {
                .Tipo = If(saldo >= 0, "SUPERAVIT", "DEFICIT"),
                .Fecha = "",
                .Concepto = "SALDO  (Entradas - Salidas)",
                .Factura = "",
                .Vinculo = "",
                .Entrada = saldo.ToString("N2") & " L",
                .Salida = If(saldo >= 0, "SUPERÁVIT", "DÉFICIT")
            })

            Return rows
        End Function

        Private Function BuildDataTable(rows As List(Of CajaRow)) As DataTable
            Dim dt As New DataTable("DataSet1")
            dt.Columns.Add("Tipo")
            dt.Columns.Add("Fecha")
            dt.Columns.Add("Concepto")
            dt.Columns.Add("Factura")
            dt.Columns.Add("Vinculo")
            dt.Columns.Add("Entrada")
            dt.Columns.Add("Salida")
            For Each row In rows
                dt.Rows.Add(row.Tipo, row.Fecha, row.Concepto, row.Factura, row.Vinculo, row.Entrada, row.Salida)
            Next
            Return dt
        End Function

        Private Function BuildRdlcXml(items As List(Of CajaChica),
                                       fechaDesde As String,
                                       fechaHasta As String,
                                       allItems As List(Of CajaChica)) As String
            Const pageW As Double = 11.0
            Const pageH As Double = 8.5
            Const margin As Double = 0.5
            ' El ancho del cuerpo + márgenes debe ser ESTRICTAMENTE menor que pageW.
            ' Si es exactamente igual (10.0 + 0.5 + 0.5 = 11.0), RDLC considera que el
            ' contenido llega al borde imprimible y emite una segunda página en blanco.
            Const cW As Double = 9.9

            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim periodo = If(String.IsNullOrWhiteSpace(fechaDesde) AndAlso String.IsNullOrWhiteSpace(fechaHasta),
                             "Todos los registros",
                             $"Período: {fechaDesde} — {fechaHasta}")
            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)

            ' Totales del período (filtrado)
            Dim totalEntradas = items.Where(Function(i) i.TipoOperacion = "Entrada").Sum(Function(i) i.Monto)
            Dim totalSalidas = items.Where(Function(i) i.TipoOperacion = "Salida").Sum(Function(i) i.Monto)
            Dim saldo = totalEntradas - totalSalidas

            ' Totales globales (estado actual real de la caja)
            Dim all = If(allItems, items)
            Dim allEnt = all.Where(Function(i) i.TipoOperacion = "Entrada").Sum(Function(i) i.Monto)
            Dim allSal = all.Where(Function(i) i.TipoOperacion = "Salida").Sum(Function(i) i.Monto)
            Dim allSaldo = allEnt - allSal
            Dim allSaldoHex = If(allSaldo >= 0, "#16A34A", "#DC2626")
            Dim esPeriodoFiltrado = allItems IsNot Nothing AndAlso
                                    Not (String.IsNullOrWhiteSpace(fechaDesde) AndAlso String.IsNullOrWhiteSpace(fechaHasta))

            ' Expresiones condicionales RDLC
            Dim bgExpr = "=IIF(Fields!Tipo.Value=""TOTAL"",""#EFF6FF"",IIF(Fields!Tipo.Value=""SUPERAVIT"",""#F0FDF4"",IIF(Fields!Tipo.Value=""DEFICIT"",""#FFF1F2"",IIF(Fields!Tipo.Value=""Entrada"",""#F0FDF4"",""White""))))"
            Dim boldExpr = "=IIF(Fields!Tipo.Value=""TOTAL"" OR Fields!Tipo.Value=""SUPERAVIT"" OR Fields!Tipo.Value=""DEFICIT"",""Bold"",""Normal"")"
            Dim sizeExpr = "=IIF(Fields!Tipo.Value=""SUPERAVIT"" OR Fields!Tipo.Value=""DEFICIT"",""11pt"",""9pt"")"
            Dim entradaColorExpr = "=IIF(Fields!Tipo.Value=""TOTAL"" OR Fields!Tipo.Value=""SUPERAVIT"" OR Fields!Tipo.Value=""DEFICIT"",""#16A34A"",""#1F2937"")"
            Dim salidaColorExpr = "=IIF(Fields!Tipo.Value=""TOTAL"" OR Fields!Tipo.Value=""DEFICIT"",""#DC2626"",IIF(Fields!Tipo.Value=""SUPERAVIT"",""#16A34A"",""#1F2937""))"

            ' Anchos de columna
            ' La suma de anchos de columna (9.9) debe coincidir con cW para no exceder el cuerpo.
            Dim colWidths = {0.9, 2.75, 1.3, 2.15, 1.4, 1.4}
            Dim colFields = {"Fecha", "Concepto", "Factura", "Vinculo", "Entrada", "Salida"}
            Dim colHdrs = {"FECHA", "CONCEPTO", "N° FACTURA", "VÍNCULO", "ENTRADA (L)", "SALIDA (L)"}

            Dim sb As New StringBuilder()
            sb.Append("<?xml version=""1.0"" encoding=""utf-8""?>")
            sb.Append("<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" ")
            sb.Append("xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">")

            ' EmbeddedImages
            If hasLogo Then
                Dim bannerB64 = ReportHeaderHelper.GetBannerBase64()
                sb.Append($"<EmbeddedImages><EmbeddedImage Name=""VemarLogo""><MIMEType>image/png</MIMEType><ImageData>{logoB64}</ImageData></EmbeddedImage><EmbeddedImage Name=""VemarBanner""><MIMEType>image/jpeg</MIMEType><ImageData>{bannerB64}</ImageData></EmbeddedImage></EmbeddedImages>")
            End If

            ' DataSources + DataSets
            sb.Append("<DataSources><DataSource Name=""DataSource1""><ConnectionProperties>")
            sb.Append("<DataProvider>System.Data.DataSet</DataProvider>")
            sb.Append("<ConnectString>/* Local Connection */</ConnectString>")
            sb.Append("</ConnectionProperties></DataSource></DataSources>")

            sb.Append("<DataSets><DataSet Name=""DataSet1""><Query>")
            sb.Append("<DataSourceName>DataSource1</DataSourceName>")
            sb.Append("<CommandText>/* Local Query */</CommandText></Query><Fields>")
            For Each f In {"Tipo", "Fecha", "Concepto", "Factura", "Vinculo", "Entrada", "Salida"}
                sb.Append($"<Field Name=""{f}""><DataField>{f}</DataField></Field>")
            Next
            sb.Append("</Fields></DataSet></DataSets>")

            ' Body
            sb.Append("<Body><ReportItems>")

            ' ── Membrete ────────────────────────────────────────────────────
            Dim hdr = ReportHeaderHelper.BuildHeader(logoB64, cW, "cc")
            sb.Append(hdr.xml)

            AppendTxt(sb, "TxtTitulo", "ESTADO DE CAJA CHICA", FmtIn(hdr.heightUsed + 0.05), "0in", "0.25in", FmtIn(cW), "11pt", "Bold", "#1E40AF")
            AppendTxt(sb, "TxtPeriodo", periodo, FmtIn(hdr.heightUsed + 0.30), "0in", "0.18in", FmtIn(cW), "9pt", "Bold", "#374151")
            AppendTxt(sb, "TxtFecha", $"Generado: {fechaGen}     Registros en período: {items.Count}",
                      FmtIn(hdr.heightUsed + 0.48), "0in", "0.16in", FmtIn(cW), "7.5pt", "Normal", "#64748B")

            ' ── Panel "Estado Actual de Caja Chica" (siempre muestra totales globales) ──
            Dim panY = hdr.heightUsed + 0.70
            Dim panH = 0.52
            Dim colW4 = cW / 4.0

            ' Fondo del panel
            sb.Append($"<Rectangle Name=""RctPanel"">")
            sb.Append($"<Top>{FmtIn(panY)}</Top><Left>0in</Left><Height>{FmtIn(panH)}</Height><Width>{FmtIn(cW)}</Width>")
            sb.Append("<Style><BackgroundColor>#EFF6FF</BackgroundColor>")
            sb.Append("<Border><Color>#BFDBFE</Color><Style>Solid</Style><Width>1pt</Width></Border></Style></Rectangle>")

            ' Título del panel
            AppendTxt(sb, "PanTit", "ESTADO ACTUAL DE CAJA CHICA",
                      FmtIn(panY + 0.03), "0.12in", "0.18in", FmtIn(cW - 0.2), "8.5pt", "Bold", "#1E40AF")

            ' 4 columnas: Total Entradas | Total Salidas | Saldo Actual | Movimientos
            Dim lblY = panY + 0.22
            Dim valY = panY + 0.33

            AppendTxt(sb, "PL1", "Total Entradas",       FmtIn(lblY), FmtIn(colW4 * 0),       "0.14in", FmtIn(colW4), "7pt", "Normal", "#64748B")
            AppendTxt(sb, "PL2", "Total Salidas",         FmtIn(lblY), FmtIn(colW4 * 1),       "0.14in", FmtIn(colW4), "7pt", "Normal", "#64748B")
            AppendTxt(sb, "PL3", "Saldo Actual",          FmtIn(lblY), FmtIn(colW4 * 2),       "0.14in", FmtIn(colW4), "7pt", "Normal", "#64748B")
            AppendTxt(sb, "PL4", "Total Movimientos",     FmtIn(lblY), FmtIn(colW4 * 3),       "0.14in", FmtIn(colW4), "7pt", "Normal", "#64748B")

            ' Ancho = colW4 - 0.12 para compensar la sangría y no exceder el borde derecho del cuerpo.
            AppendTxt(sb, "PV1", $"L {allEnt:N2}",       FmtIn(valY), FmtIn(colW4 * 0 + 0.12), "0.16in", FmtIn(colW4 - 0.12), "9pt", "Bold", "#16A34A")
            AppendTxt(sb, "PV2", $"L {allSal:N2}",       FmtIn(valY), FmtIn(colW4 * 1 + 0.12), "0.16in", FmtIn(colW4 - 0.12), "9pt", "Bold", "#DC2626")
            AppendTxt(sb, "PV3", $"L {allSaldo:N2}",     FmtIn(valY), FmtIn(colW4 * 2 + 0.12), "0.16in", FmtIn(colW4 - 0.12), "9pt", "Bold", allSaldoHex)
            AppendTxt(sb, "PV4", all.Count.ToString(),   FmtIn(valY), FmtIn(colW4 * 3 + 0.12), "0.16in", FmtIn(colW4 - 0.12), "9pt", "Bold", "#374151")

            ' Separador si hay filtro de período
            Dim periodoLabel = If(esPeriodoFiltrado, $"DETALLE DEL PERÍODO: {periodo}", "DETALLE DE TODOS LOS REGISTROS")
            AppendSep(sb, "RctSep2", FmtIn(panY + panH + 0.06), FmtIn(cW), "#CBD5E1")
            AppendTxt(sb, "TxtPerLbl", periodoLabel,
                      FmtIn(panY + panH + 0.10), "0in", "0.18in", FmtIn(cW), "8pt", "Bold", "#374151")
            AppendTxt(sb, "TxtPerSub", $"Entradas: L {totalEntradas:N2}   |   Salidas: L {totalSalidas:N2}   |   Saldo período: L {saldo:N2}   |   Registros: {items.Count}",
                      FmtIn(panY + panH + 0.28), "0in", "0.16in", FmtIn(cW), "7.5pt", "Normal", "#64748B")

            Dim tablixTop = panY + panH + 0.28 + 0.16 + 0.10

            ' Tablix
            sb.Append("<Tablix Name=""Tablix1""><TablixBody>")
            sb.Append("<TablixColumns>")
            For Each w In colWidths
                sb.Append($"<TablixColumn><Width>{FmtIn(w)}</Width></TablixColumn>")
            Next
            sb.Append("</TablixColumns><TablixRows>")

            ' Fila encabezados
            sb.Append("<TablixRow><Height>0.32in</Height><TablixCells>")
            For i = 0 To colHdrs.Length - 1
                sb.Append("<TablixCell><CellContents>")
                sb.Append($"<Textbox Name=""Hdr{i}""><CanGrow>true</CanGrow>")
                sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
                sb.Append($"<Value>{XmlEsc(colHdrs(i))}</Value>")
                sb.Append("<Style><FontWeight>Bold</FontWeight><Color>White</Color><FontSize>9.5pt</FontSize></Style>")
                sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
                sb.Append("<Style><BackgroundColor>#1E40AF</BackgroundColor><VerticalAlign>Middle</VerticalAlign>")
                sb.Append("<PaddingLeft>6pt</PaddingLeft><PaddingRight>5pt</PaddingRight>")
                sb.Append("<PaddingTop>6pt</PaddingTop><PaddingBottom>6pt</PaddingBottom>")
                sb.Append("</Style></Textbox></CellContents></TablixCell>")
            Next
            sb.Append("</TablixCells></TablixRow>")

            ' Fila datos
            sb.Append("<TablixRow><Height>0.30in</Height><TablixCells>")
            For i = 0 To colFields.Length - 1
                Dim fld = colFields(i)
                Dim colorExpr As String
                Select Case fld
                    Case "Entrada" : colorExpr = entradaColorExpr
                    Case "Salida" : colorExpr = salidaColorExpr
                    Case Else : colorExpr = "=IIF(Fields!Tipo.Value=""TOTAL"" OR Fields!Tipo.Value=""SUPERAVIT"" OR Fields!Tipo.Value=""DEFICIT"",""#1E40AF"",""#1F2937"")"
                End Select

                sb.Append("<TablixCell><CellContents>")
                sb.Append($"<Textbox Name=""Val{i}""><CanGrow>true</CanGrow>")
                sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
                sb.Append($"<Value>=Fields!{fld}.Value</Value>")
                sb.Append("<Style>")
                sb.Append($"<FontSize>{sizeExpr}</FontSize>")
                sb.Append($"<FontWeight>{boldExpr}</FontWeight>")
                sb.Append($"<Color>{colorExpr}</Color>")
                sb.Append("</Style>")
                sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
                sb.Append("<Style>")
                sb.Append($"<BackgroundColor>{bgExpr}</BackgroundColor><VerticalAlign>Middle</VerticalAlign>")
                sb.Append("<BottomBorder><Color>#E2E8F0</Color><Style>Solid</Style><Width>0.5pt</Width></BottomBorder>")
                sb.Append("<PaddingLeft>6pt</PaddingLeft><PaddingRight>5pt</PaddingRight>")
                sb.Append("<PaddingTop>5pt</PaddingTop><PaddingBottom>5pt</PaddingBottom>")
                sb.Append("</Style></Textbox></CellContents></TablixCell>")
            Next
            sb.Append("</TablixCells></TablixRow>")
            sb.Append("</TablixRows></TablixBody>")

            sb.Append("<TablixColumnHierarchy><TablixMembers>")
            For i = 0 To colFields.Length - 1
                sb.Append("<TablixMember/>")
            Next
            sb.Append("</TablixMembers></TablixColumnHierarchy>")

            sb.Append("<TablixRowHierarchy><TablixMembers>")
            sb.Append("<TablixMember><KeepWithGroup>After</KeepWithGroup><RepeatOnNewPage>true</RepeatOnNewPage></TablixMember>")
            sb.Append("<TablixMember><Group Name=""Details""/></TablixMember>")
            sb.Append("</TablixMembers></TablixRowHierarchy>")

            sb.Append("<DataSetName>DataSet1</DataSetName>")
            sb.Append($"<Top>{FmtIn(tablixTop)}</Top><Left>0in</Left>")
            sb.Append($"<Height>0.5in</Height><Width>{FmtIn(cW)}</Width></Tablix>")

            ' Estimar líneas extra por wrap en Concepto (2.8in ≈ 44 car./línea) y Vínculo (2.2in ≈ 33 car./línea),
            ' para que el cálculo de altura refleje el alto real de filas con texto largo y no se generen páginas
            ' adicionales casi en blanco por subestimar el contenido.
            Dim wrapExtraH As Double = 0
            For Each item In items
                Dim conceptoLineas = Math.Max(1, CInt(Math.Ceiling(If(item.Concepto, "").Length / 44.0)))
                Dim vinculoLineas = Math.Max(1, CInt(Math.Ceiling(GetVinculo(item).Length / 33.0)))
                Dim lineasFila = Math.Max(conceptoLineas, vinculoLineas)
                If lineasFila > 1 Then wrapExtraH += (lineasFila - 1) * 0.16
            Next

            Dim ccContentH As Double = tablixTop + 0.32 + ((items.Count + 2) * 0.30) + wrapExtraH + 0.10
            Dim ccMaxBodyH As Double = pageH - margin * 2 - 0.02
            sb.Append($"</ReportItems><Height>{FmtIn(Math.Min(ccContentH, ccMaxBodyH))}</Height></Body>")
            sb.Append($"<Width>{FmtIn(cW)}</Width>")
            sb.Append("<Page>")
            sb.Append($"<PageHeight>{FmtIn(pageH)}</PageHeight><PageWidth>{FmtIn(pageW)}</PageWidth>")
            sb.Append($"<LeftMargin>{FmtIn(margin)}</LeftMargin><RightMargin>{FmtIn(margin)}</RightMargin>")
            sb.Append("<TopMargin>0.50in</TopMargin><BottomMargin>0.50in</BottomMargin>")
            sb.Append("</Page><Language>es-HN</Language>")
            sb.Append("<ConsumeContainerWhitespace>true</ConsumeContainerWhitespace>")
            sb.Append("</Report>")

            Return sb.ToString()
        End Function

        Private Sub AppendTxt(sb As StringBuilder, name As String, value As String,
                              top As String, left As String, height As String, width As String,
                              fontSize As String, fontWeight As String, color As String)
            sb.Append($"<Textbox Name=""{name}""><CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
            sb.Append($"<Value>{XmlEsc(value)}</Value>")
            sb.Append($"<Style><FontSize>{fontSize}</FontSize><FontWeight>{fontWeight}</FontWeight><Color>{color}</Color></Style>")
            sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width></Textbox>")
        End Sub

        Private Sub AppendSep(sb As StringBuilder, name As String, top As String, width As String, color As String)
            sb.Append($"<Rectangle Name=""{name}"">")
            sb.Append($"<Top>{top}</Top><Left>0in</Left><Height>0.01in</Height><Width>{width}</Width>")
            sb.Append($"<Style><BottomBorder><Color>{color}</Color><Style>Solid</Style><Width>1pt</Width></BottomBorder></Style></Rectangle>")
        End Sub

        Private Function FmtIn(d As Double) As String
            Return d.ToString("F2", CultureInfo.InvariantCulture) & "in"
        End Function

        Private Function XmlEsc(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function

        Private Function BuildMsg(ex As Exception) As String
            Dim msg = ex.Message
            Dim inner = ex.InnerException
            Do While inner IsNot Nothing
                msg &= " → " & inner.Message
                inner = inner.InnerException
            Loop
            Return msg
        End Function

    End Class

End Namespace
