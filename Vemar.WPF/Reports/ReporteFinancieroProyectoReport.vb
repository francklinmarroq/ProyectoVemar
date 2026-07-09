Imports System.Data
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports ClosedXML.Excel
Imports Microsoft.Reporting.NETCore
Imports Vemar.Domain

Namespace Vemar.WPF.Reports

    Friend Class EgresoRow
        Public Property Tipo As String
        Public Property Col1 As String
        Public Property Col2 As String
        Public Property Col3 As String
        Public Property Col4 As String
        Public Property Col5 As String
    End Class

    Public Class ReporteFinancieroProyectoReport

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

        Public Async Function GeneratePdfAsync(proyecto As Proyecto, gastos As List(Of GastoProyecto), pagos As List(Of PagoContrato)) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rows = BuildRows(gastos, pagos)
                    Dim dt = BuildDataTable(rows)
                    Dim rdlcXml = BuildRdlcXml(proyecto)

                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using
                    report.DataSources.Add(New ReportDataSource("DataSet1", dt))

                    Dim pdfBytes = report.Render("PDF")
                    Dim nombre = If(String.IsNullOrWhiteSpace(proyecto.Nombre), "proyecto", proyecto.Nombre)
                    PdfPreviewHelper.ShowPreview(pdfBytes, "Reporte Financiero Proyecto", $"ReporteFinanciero_{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
                Catch ex As Exception
                    Dim msg = BuildErrorMsg(ex)
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar PDF: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        Public Async Function GenerateExcelAsync(proyecto As Proyecto, gastos As List(Of GastoProyecto), pagos As List(Of PagoContrato)) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim nombre = If(String.IsNullOrWhiteSpace(proyecto.Nombre), "proyecto", proyecto.Nombre)
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"ReporteFinanciero_{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx")

                    Using wb As New XLWorkbook()
                        Dim ws = wb.Worksheets.Add("Reporte Financiero")
                        Dim r As Integer = 1
                        Const totalCols = 5

                        ' ── Encabezado empresa ──────────────────────────────────────────
                        ws.Cell(r, 1).Value = "CONSTRUCTORA VEMAR S. de R.L. de C.V."
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True : .FontSize = 14 : .FontColor = XLColor.FromHtml("#1E3A8A")
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        ws.Cell(r, 1).Value = "REPORTE FINANCIERO DE PROYECTO"
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True : .FontSize = 12 : .FontColor = XLColor.FromHtml("#1E40AF")
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        ws.Cell(r, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}"
                        With ws.Cell(r, 1).Style.Font
                            .Italic = True : .FontColor = XLColor.FromHtml("#64748B") : .FontSize = 10
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        Dim sep = ws.Range(r, 1, r, totalCols)
                        sep.Merge()
                        sep.Style.Border.BottomBorder = XLBorderStyleValues.Medium
                        sep.Style.Border.BottomBorderColor = XLColor.FromHtml("#1E40AF")
                        r += 1

                        ' ── Datos del proyecto ──────────────────────────────────────────
                        Dim addInfo = Sub(lbl As String, val As String)
                                          ws.Cell(r, 1).Value = lbl
                                          With ws.Cell(r, 1).Style.Font
                                              .Bold = True : .FontColor = XLColor.FromHtml("#1E40AF")
                                          End With
                                          ws.Cell(r, 2).Value = val
                                          ws.Range(r, 2, r, totalCols).Merge()
                                          r += 1
                                      End Sub

                        addInfo("Proyecto:", proyecto.Nombre)
                        addInfo("Cliente:", If(proyecto.Cliente?.Nombre, "—"))
                        addInfo("Ubicación:", proyecto.Ubicacion)
                        addInfo("Clave SURE:", proyecto.ClaveSure)
                        addInfo("Matrícula:", proyecto.Matricula)
                        addInfo("Área:", proyecto.Area.ToString("N2") & " m²")
                        addInfo("Categoría:", If(proyecto.CategoriaProyecto?.Nombre, "—"))
                        addInfo("Zonificación:", If(proyecto.Zonificacion?.Zonificacion, "—"))
                        r += 1

                        ' ── GASTOS ──────────────────────────────────────────────────────
                        ws.Range(r, 1, r, totalCols).Merge()
                        ws.Cell(r, 1).Value = "GASTOS"
                        ws.Range(r, 1, r, totalCols).Style.Fill.BackgroundColor = XLColor.FromHtml("#EA580C")
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True : .FontSize = 11 : .FontColor = XLColor.White
                        End With
                        r += 1

                        Dim ghdr = {"Descripción", "Cantidad", "Costo Unit.", "Total", "Estado"}
                        For c = 1 To 5
                            ws.Cell(r, c).Value = ghdr(c - 1)
                            With ws.Cell(r, c).Style
                                .Font.Bold = True
                                .Fill.BackgroundColor = XLColor.FromHtml("#FFF7ED")
                                .Font.FontColor = XLColor.FromHtml("#C2410C")
                            End With
                        Next
                        r += 1

                        Dim totalGastos As Decimal = 0
                        For Each g In gastos
                            Dim total = g.Cantidad * g.CostoUnitario
                            totalGastos += total
                            ws.Cell(r, 1).Value = g.Descripcion
                            ws.Cell(r, 2).Value = g.Cantidad.ToString("N2")
                            ws.Cell(r, 3).Value = g.CostoUnitario.ToString("N2")
                            ws.Cell(r, 4).Value = total.ToString("N2")
                            ws.Cell(r, 5).Value = If(g.PendienteDePago, "Pendiente", "Pagado")
                            ws.Cell(r, 5).Style.Font.FontColor = If(g.PendienteDePago, XLColor.FromHtml("#DC2626"), XLColor.FromHtml("#16A34A"))
                            r += 1
                        Next
                        If gastos.Count = 0 Then
                            ws.Cell(r, 1).Value = "(Sin gastos registrados)"
                            ws.Cell(r, 1).Style.Font.Italic = True
                            ws.Cell(r, 1).Style.Font.FontColor = XLColor.FromHtml("#94A3B8")
                            ws.Range(r, 1, r, totalCols).Merge()
                            r += 1
                        End If

                        Dim gtRow = ws.Range(r, 1, r, totalCols)
                        gtRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC")
                        gtRow.Style.Border.TopBorder = XLBorderStyleValues.Medium
                        gtRow.Style.Border.TopBorderColor = XLColor.FromHtml("#EA580C")
                        ws.Cell(r, 3).Value = "TOTAL GASTOS:"
                        ws.Cell(r, 3).Style.Font.Bold = True
                        ws.Cell(r, 4).Value = totalGastos.ToString("N2") & " L"
                        With ws.Cell(r, 4).Style.Font
                            .Bold = True : .FontColor = XLColor.FromHtml("#DC2626")
                        End With
                        r += 2

                        ' ── PAGOS DE CONTRATOS ──────────────────────────────────────────
                        ws.Range(r, 1, r, totalCols).Merge()
                        ws.Cell(r, 1).Value = "PAGOS DE CONTRATOS"
                        ws.Range(r, 1, r, totalCols).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E40AF")
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True : .FontSize = 11 : .FontColor = XLColor.White
                        End With
                        r += 1

                        Dim phdr = {"Contratista", "Descripción", "Valor", "", ""}
                        For c = 1 To 3
                            ws.Cell(r, c).Value = phdr(c - 1)
                            With ws.Cell(r, c).Style
                                .Font.Bold = True
                                .Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF")
                                .Font.FontColor = XLColor.FromHtml("#1E40AF")
                            End With
                        Next
                        ws.Range(r, 4, r, totalCols).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF")
                        r += 1

                        Dim totalPagos As Decimal = 0
                        For Each p In pagos
                            totalPagos += p.Valor
                            ws.Cell(r, 1).Value = If(p.Contrato?.Contratista?.Nombre, "—")
                            ws.Cell(r, 2).Value = p.Descripcion
                            ws.Cell(r, 3).Value = p.Valor.ToString("N2") & " L"
                            r += 1
                        Next
                        If pagos.Count = 0 Then
                            ws.Cell(r, 1).Value = "(Sin pagos de contratos registrados)"
                            ws.Cell(r, 1).Style.Font.Italic = True
                            ws.Cell(r, 1).Style.Font.FontColor = XLColor.FromHtml("#94A3B8")
                            ws.Range(r, 1, r, totalCols).Merge()
                            r += 1
                        End If

                        Dim ptRow = ws.Range(r, 1, r, totalCols)
                        ptRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC")
                        ptRow.Style.Border.TopBorder = XLBorderStyleValues.Medium
                        ptRow.Style.Border.TopBorderColor = XLColor.FromHtml("#1E40AF")
                        ws.Cell(r, 2).Value = "TOTAL PAGOS:"
                        ws.Cell(r, 2).Style.Font.Bold = True
                        ws.Cell(r, 3).Value = totalPagos.ToString("N2") & " L"
                        With ws.Cell(r, 3).Style.Font
                            .Bold = True : .FontColor = XLColor.FromHtml("#1E40AF")
                        End With
                        r += 2

                        ' ── TOTAL EGRESOS ───────────────────────────────────────────────
                        Dim totalEgresos = totalGastos + totalPagos
                        Dim teRow = ws.Range(r, 1, r, totalCols)
                        teRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#FEE2E2")
                        teRow.Style.Border.TopBorder = XLBorderStyleValues.Double
                        teRow.Style.Border.TopBorderColor = XLColor.FromHtml("#DC2626")

                        ws.Cell(r, 1).Value = "TOTAL EGRESOS  (Gastos + Pagos de Contratos):"
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True : .FontSize = 11 : .FontColor = XLColor.FromHtml("#991B1B")
                        End With
                        ws.Range(r, 1, r, 3).Merge()

                        ws.Cell(r, 4).Value = totalEgresos.ToString("N2") & " L"
                        With ws.Cell(r, 4).Style.Font
                            .Bold = True : .FontSize = 11 : .FontColor = XLColor.FromHtml("#DC2626")
                        End With

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

        ' ─── Lista plana de filas ───────────────────────────────────────────
        Private Function BuildRows(gastos As List(Of GastoProyecto), pagos As List(Of PagoContrato)) As List(Of EgresoRow)
            Dim rows As New List(Of EgresoRow)()

            ' === GASTOS ===
            rows.Add(New EgresoRow With {.Tipo = "SECTION_G", .Col1 = "GASTOS", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            rows.Add(New EgresoRow With {.Tipo = "HDR_G", .Col1 = "Descripción", .Col2 = "Cantidad", .Col3 = "Costo Unit.", .Col4 = "Total", .Col5 = "Estado"})

            Dim totalGastos As Decimal = 0
            For Each g In gastos
                Dim total = g.Cantidad * g.CostoUnitario
                totalGastos += total
                rows.Add(New EgresoRow With {
                    .Tipo = "GASTO",
                    .Col1 = g.Descripcion,
                    .Col2 = g.Cantidad.ToString("N2"),
                    .Col3 = g.CostoUnitario.ToString("N2"),
                    .Col4 = total.ToString("N2"),
                    .Col5 = If(g.PendienteDePago, "Pendiente", "Pagado")
                })
            Next
            If gastos.Count = 0 Then
                rows.Add(New EgresoRow With {.Tipo = "EMPTY", .Col1 = "(Sin gastos registrados)", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            End If
            rows.Add(New EgresoRow With {.Tipo = "TOTAL_G", .Col1 = "", .Col2 = "", .Col3 = "TOTAL GASTOS:", .Col4 = totalGastos.ToString("N2") & " L", .Col5 = ""})

            rows.Add(New EgresoRow With {.Tipo = "SPACER", .Col1 = "", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})

            ' === PAGOS DE CONTRATOS ===
            rows.Add(New EgresoRow With {.Tipo = "SECTION_P", .Col1 = "PAGOS DE CONTRATOS", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            rows.Add(New EgresoRow With {.Tipo = "HDR_P", .Col1 = "Contratista", .Col2 = "Descripción", .Col3 = "Valor", .Col4 = "", .Col5 = ""})

            Dim totalPagos As Decimal = 0
            For Each p In pagos
                totalPagos += p.Valor
                rows.Add(New EgresoRow With {
                    .Tipo = "PAGO",
                    .Col1 = If(p.Contrato?.Contratista?.Nombre, "—"),
                    .Col2 = p.Descripcion,
                    .Col3 = p.Valor.ToString("N2") & " L",
                    .Col4 = "",
                    .Col5 = ""
                })
            Next
            If pagos.Count = 0 Then
                rows.Add(New EgresoRow With {.Tipo = "EMPTY", .Col1 = "(Sin pagos de contratos registrados)", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            End If
            rows.Add(New EgresoRow With {.Tipo = "TOTAL_P", .Col1 = "", .Col2 = "TOTAL PAGOS:", .Col3 = totalPagos.ToString("N2") & " L", .Col4 = "", .Col5 = ""})

            rows.Add(New EgresoRow With {.Tipo = "SPACER", .Col1 = "", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})

            ' === TOTAL EGRESOS ===
            Dim totalEgresos = totalGastos + totalPagos
            rows.Add(New EgresoRow With {
                .Tipo = "TOTAL_E",
                .Col1 = "TOTAL EGRESOS  (Gastos + Pagos de Contratos)",
                .Col2 = "",
                .Col3 = "",
                .Col4 = totalEgresos.ToString("N2") & " L",
                .Col5 = ""
            })

            Return rows
        End Function

        Private Function BuildDataTable(rows As List(Of EgresoRow)) As DataTable
            Dim dt As New DataTable("DataSet1")
            dt.Columns.Add("Tipo")
            dt.Columns.Add("Col1")
            dt.Columns.Add("Col2")
            dt.Columns.Add("Col3")
            dt.Columns.Add("Col4")
            dt.Columns.Add("Col5")
            For Each row In rows
                dt.Rows.Add(row.Tipo, row.Col1, row.Col2, row.Col3, row.Col4, row.Col5)
            Next
            Return dt
        End Function

        ' ─── RDLC dinámico ─────────────────────────────────────────────────
        Private Function BuildRdlcXml(proyecto As Proyecto) As String
            Const pageW As Double = 8.5
            Const pageH As Double = 11.0
            Const margin As Double = 0.5
            Const cW As Double = 7.5
            Const ibRowH As Double = 0.22
            Const ibLeftW As Double = 3.5
            Const ibRightX As Double = 3.9
            Const ibRightW As Double = 3.6
            Const ibRows As Integer = 4

            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)

            Dim hdr = ReportHeaderHelper.BuildHeader(logoB64, cW, "rfp")

            Dim ibTop As Double = hdr.heightUsed + 0.56
            Dim objetoTop As Double = ibTop + ibRows * ibRowH + 0.02
            Dim sep2Top As Double = objetoTop + ibRowH + 0.04
            Dim tablixTop As Double = sep2Top + 0.12

            ' Expresiones condicionales (colores por tipo de fila)
            Dim bgExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"",""#EA580C"",IIF(Fields!Tipo.Value=""SECTION_P"",""#1E40AF"",IIF(Fields!Tipo.Value=""HDR_G"",""#FFF7ED"",IIF(Fields!Tipo.Value=""HDR_P"",""#EFF6FF"",IIF(Fields!Tipo.Value=""TOTAL_G"" OR Fields!Tipo.Value=""TOTAL_P"",""#F8FAFC"",IIF(Fields!Tipo.Value=""TOTAL_E"",""#FEE2E2"",""White""))))))"
            Dim colorExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_P"",""White"",IIF(Fields!Tipo.Value=""HDR_G"",""#C2410C"",IIF(Fields!Tipo.Value=""HDR_P"",""#1E40AF"",IIF(Fields!Tipo.Value=""TOTAL_E"",""#991B1B"",""#1F2937""))))"
            Dim boldExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_P"" OR Fields!Tipo.Value=""HDR_G"" OR Fields!Tipo.Value=""HDR_P"" OR Fields!Tipo.Value=""TOTAL_G"" OR Fields!Tipo.Value=""TOTAL_P"" OR Fields!Tipo.Value=""TOTAL_E"",""Bold"",""Normal"")"
            Dim sizeExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_P"" OR Fields!Tipo.Value=""TOTAL_E"",""10pt"",""8pt"")"

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

            sb.Append("<DataSources><DataSource Name=""DataSource1""><ConnectionProperties>")
            sb.Append("<DataProvider>System.Data.DataSet</DataProvider>")
            sb.Append("<ConnectString>/* Local Connection */</ConnectString>")
            sb.Append("</ConnectionProperties></DataSource></DataSources>")

            sb.Append("<DataSets><DataSet Name=""DataSet1"">")
            sb.Append("<Query><DataSourceName>DataSource1</DataSourceName>")
            sb.Append("<CommandText>/* Local Query */</CommandText></Query><Fields>")
            For Each f In {"Tipo", "Col1", "Col2", "Col3", "Col4", "Col5"}
                sb.Append($"<Field Name=""{f}""><DataField>{f}</DataField></Field>")
            Next
            sb.Append("</Fields></DataSet></DataSets>")

            sb.Append("<Body><ReportItems>")

            ' ── Membrete ────────────────────────────────────────────────────
            sb.Append(hdr.xml)

            AppendTxt(sb, "TxtTitulo", "REPORTE FINANCIERO DE PROYECTO", FmtIn(hdr.heightUsed + 0.05), "0in", "0.25in", FmtIn(cW), "11pt", "Bold", "#1E40AF")
            AppendTxt(sb, "TxtFecha", $"Generado: {fechaGen}", FmtIn(hdr.heightUsed + 0.30), "0in", "0.18in", FmtIn(cW), "8pt", "Normal", "#64748B")

            ' Datos del proyecto (4 filas x 2 columnas)
            Dim leftData = New (String, String)() {
                ("Proyecto:", proyecto.Nombre),
                ("Ubicación:", proyecto.Ubicacion),
                ("Clave SURE:", proyecto.ClaveSure),
                ("Categoría:", If(proyecto.CategoriaProyecto?.Nombre, "—"))
            }
            Dim rightData = New (String, String)() {
                ("Cliente:", If(proyecto.Cliente?.Nombre, "—")),
                ("Área:", proyecto.Area.ToString("N2") & " m²"),
                ("Matrícula:", proyecto.Matricula),
                ("Zonificación:", If(proyecto.Zonificacion?.Zonificacion, "—"))
            }
            For i = 0 To ibRows - 1
                Dim topFmt = FmtIn(ibTop + i * ibRowH)
                AppendInfoTxt(sb, $"InfoL{i}", leftData(i).Item1, leftData(i).Item2, topFmt, "0in", FmtIn(ibRowH), FmtIn(ibLeftW))
                AppendInfoTxt(sb, $"InfoR{i}", rightData(i).Item1, rightData(i).Item2, topFmt, FmtIn(ibRightX), FmtIn(ibRowH), FmtIn(ibRightW))
            Next

            ' Fila extra: descripción amplia si hay nombre largo
            AppendInfoTxt(sb, "InfoExtra", "ID:", proyecto.Id.ToString(), FmtIn(objetoTop), "0in", FmtIn(ibRowH), FmtIn(cW))

            AppendSep(sb, "RctSep2", FmtIn(sep2Top), FmtIn(cW), "#94A3B8")

            ' Tablix
            Dim colWidths = {2.8, 1.5, 1.0, 1.2, 1.0}
            Dim colFields = {"Col1", "Col2", "Col3", "Col4", "Col5"}

            sb.Append("<Tablix Name=""Tablix1""><TablixBody>")
            sb.Append("<TablixColumns>")
            For Each w In colWidths
                sb.Append($"<TablixColumn><Width>{FmtIn(w)}</Width></TablixColumn>")
            Next
            sb.Append("</TablixColumns><TablixRows>")

            sb.Append("<TablixRow><Height>0.26in</Height><TablixCells>")
            For i = 0 To 4
                sb.Append("<TablixCell><CellContents>")
                sb.Append($"<Textbox Name=""DC{i}""><CanGrow>true</CanGrow>")
                sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
                sb.Append($"<Value>=Fields!{colFields(i)}.Value</Value>")
                sb.Append("<Style>")
                sb.Append($"<FontWeight>{boldExpr}</FontWeight>")
                sb.Append($"<FontSize>{sizeExpr}</FontSize>")
                sb.Append($"<Color>{colorExpr}</Color>")
                sb.Append("</Style>")
                sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
                sb.Append("<Style>")
                sb.Append($"<BackgroundColor>{bgExpr}</BackgroundColor>")
                sb.Append("<BottomBorder><Color>#E2E8F0</Color><Style>Solid</Style><Width>0.5pt</Width></BottomBorder>")
                sb.Append("<PaddingLeft>6pt</PaddingLeft><PaddingRight>4pt</PaddingRight>")
                sb.Append("<PaddingTop>3pt</PaddingTop><PaddingBottom>3pt</PaddingBottom>")
                sb.Append("</Style>")
                sb.Append("</Textbox></CellContents></TablixCell>")
            Next
            sb.Append("</TablixCells></TablixRow>")
            sb.Append("</TablixRows></TablixBody>")

            sb.Append("<TablixColumnHierarchy><TablixMembers>")
            For i = 0 To 4
                sb.Append("<TablixMember/>")
            Next
            sb.Append("</TablixMembers></TablixColumnHierarchy>")

            sb.Append("<TablixRowHierarchy><TablixMembers>")
            sb.Append("<TablixMember><Group Name=""Details""/></TablixMember>")
            sb.Append("</TablixMembers></TablixRowHierarchy>")

            sb.Append("<DataSetName>DataSet1</DataSetName>")
            sb.Append($"<Top>{FmtIn(tablixTop)}</Top><Left>0in</Left>")
            sb.Append($"<Height>0.26in</Height><Width>{FmtIn(cW)}</Width>")
            sb.Append("</Tablix>")

            sb.Append("</ReportItems><Height>10in</Height></Body>")
            sb.Append($"<Width>{FmtIn(cW)}</Width>")
            sb.Append("<Page>")
            sb.Append($"<PageHeight>{FmtIn(pageH)}</PageHeight><PageWidth>{FmtIn(pageW)}</PageWidth>")
            sb.Append($"<LeftMargin>{FmtIn(margin)}</LeftMargin><RightMargin>{FmtIn(margin)}</RightMargin>")
            sb.Append("<TopMargin>0.50in</TopMargin><BottomMargin>0.50in</BottomMargin>")
            sb.Append("</Page><Language>es-HN</Language></Report>")

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
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("</Textbox>")
        End Sub

        Private Sub AppendInfoTxt(sb As StringBuilder, name As String, label As String, value As String,
                                  top As String, left As String, height As String, width As String)
            sb.Append($"<Textbox Name=""{name}""><CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph><TextRuns>")
            sb.Append("<TextRun>")
            sb.Append($"<Value>{XmlEsc(label)}  </Value>")
            sb.Append("<Style><FontWeight>Bold</FontWeight><Color>#1E40AF</Color><FontSize>8pt</FontSize></Style>")
            sb.Append("</TextRun><TextRun>")
            sb.Append($"<Value>{XmlEsc(value)}</Value>")
            sb.Append("<Style><FontSize>8pt</FontSize><Color>#1F2937</Color></Style>")
            sb.Append("</TextRun>")
            sb.Append("</TextRuns></Paragraph></Paragraphs>")
            sb.Append($"<Top>{top}</Top><Left>{left}</Left><Height>{height}</Height><Width>{width}</Width>")
            sb.Append("</Textbox>")
        End Sub

        Private Sub AppendSep(sb As StringBuilder, name As String, top As String, width As String, color As String)
            sb.Append($"<Rectangle Name=""{name}"">")
            sb.Append($"<Top>{top}</Top><Left>0in</Left><Height>0.01in</Height><Width>{width}</Width>")
            sb.Append($"<Style><BottomBorder><Color>{color}</Color><Style>Solid</Style><Width>1pt</Width></BottomBorder></Style>")
            sb.Append("</Rectangle>")
        End Sub

        Private Function FmtIn(d As Double) As String
            Return d.ToString("F2", CultureInfo.InvariantCulture) & "in"
        End Function

        Private Function XmlEsc(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function

        Private Function BuildErrorMsg(ex As Exception) As String
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
