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

    Friend Class FinancialRow
        Public Property Tipo As String
        Public Property Col1 As String
        Public Property Col2 As String
        Public Property Col3 As String
        Public Property Col4 As String
        Public Property Col5 As String
    End Class

    Public Class ReporteFinancieroRemedidaReport

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

        Public Async Function GeneratePdfAsync(remedida As Remedida, gastos As List(Of GastoRemedida), cobros As List(Of CobroRemedida)) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim rows = BuildRows(gastos, cobros)
                    Dim dt = BuildDataTable(rows)
                    Dim rdlcXml = BuildRdlcXml(remedida)

                    Dim report As New LocalReport()
                    Using ms As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(ms)
                    End Using
                    report.DataSources.Add(New ReportDataSource("DataSet1", dt))

                    Dim pdfBytes = report.Render("PDF")
                    Dim clave = If(String.IsNullOrWhiteSpace(remedida.ClaveSure), "remedida", remedida.ClaveSure)
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"ReporteFinanciero_{clave}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf")
                    File.WriteAllBytes(filePath, pdfBytes)
                    Process.Start(New ProcessStartInfo With {.FileName = filePath, .UseShellExecute = True})
                    Return filePath
                Catch ex As Exception
                    Dim msg = BuildErrorMsg(ex)
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar PDF: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        Public Async Function GenerateExcelAsync(remedida As Remedida, gastos As List(Of GastoRemedida), cobros As List(Of CobroRemedida)) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim clave = If(String.IsNullOrWhiteSpace(remedida.ClaveSure), "remedida", remedida.ClaveSure)
                    Dim filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"ReporteFinanciero_{clave}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx")

                    Using wb As New XLWorkbook()
                        Dim ws = wb.Worksheets.Add("Reporte Financiero")
                        Dim r As Integer = 1
                        Const totalCols = 5

                        ' ── Encabezado empresa ──────────────────────────────────────────
                        ws.Cell(r, 1).Value = "CONSTRUCTORA VEMAR S. de R.L. de C.V."
                        With ws.Cell(r, 1).Style
                            .Font.Bold = True
                            .Font.FontSize = 14
                            .Font.FontColor = XLColor.FromHtml("#1E3A8A")
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        ws.Cell(r, 1).Value = "REPORTE FINANCIERO DE REMEDIDA"
                        With ws.Cell(r, 1).Style
                            .Font.Bold = True
                            .Font.FontSize = 12
                            .Font.FontColor = XLColor.FromHtml("#1E40AF")
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        ws.Cell(r, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}"
                        With ws.Cell(r, 1).Style
                            .Font.Italic = True
                            .Font.FontColor = XLColor.FromHtml("#64748B")
                            .Font.FontSize = 10
                        End With
                        ws.Range(r, 1, r, totalCols).Merge()
                        r += 1

                        Dim sepRange = ws.Range(r, 1, r, totalCols)
                        sepRange.Merge()
                        sepRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium
                        sepRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#1E40AF")
                        r += 1

                        ' ── Datos de la remedida ────────────────────────────────────────
                        Dim addInfo = Sub(lbl As String, val As String)
                                          ws.Cell(r, 1).Value = lbl
                                          With ws.Cell(r, 1).Style.Font
                                              .Bold = True
                                              .FontColor = XLColor.FromHtml("#1E40AF")
                                          End With
                                          ws.Cell(r, 2).Value = val
                                          ws.Range(r, 2, r, totalCols).Merge()
                                          r += 1
                                      End Sub

                        addInfo("Representante:", remedida.Representante)
                        addInfo("Ubicación:", remedida.Ubicacion)
                        addInfo("Clave SURE:", remedida.ClaveSure)
                        addInfo("Matrícula:", remedida.Matricula)
                        addInfo("CAM:", remedida.Cam)
                        addInfo("Objeto:", remedida.Objeto)
                        addInfo("Fecha:", remedida.Fecha.ToString("dd/MM/yyyy"))
                        addInfo("Precio Pactado:", remedida.Precio.ToString("N2") & " L")
                        addInfo("Expediente:", If(remedida.ExpedienteEntregado, "Entregado", "Pendiente"))
                        r += 1

                        ' ── GASTOS ──────────────────────────────────────────────────────
                        ws.Range(r, 1, r, totalCols).Merge()
                        ws.Cell(r, 1).Value = "GASTOS"
                        Dim gsecRange = ws.Range(r, 1, r, totalCols)
                        gsecRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#EA580C")
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True
                            .FontSize = 11
                            .FontColor = XLColor.White
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
                            With ws.Cell(r, 1).Style.Font
                                .Italic = True
                                .FontColor = XLColor.FromHtml("#94A3B8")
                            End With
                            ws.Range(r, 1, r, totalCols).Merge()
                            r += 1
                        End If

                        Dim gtotalRange = ws.Range(r, 1, r, totalCols)
                        gtotalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC")
                        gtotalRange.Style.Border.TopBorder = XLBorderStyleValues.Medium
                        gtotalRange.Style.Border.TopBorderColor = XLColor.FromHtml("#EA580C")
                        ws.Cell(r, 3).Value = "TOTAL GASTOS:"
                        ws.Cell(r, 3).Style.Font.Bold = True
                        ws.Cell(r, 4).Value = totalGastos.ToString("N2") & " L"
                        With ws.Cell(r, 4).Style.Font
                            .Bold = True
                            .FontColor = XLColor.FromHtml("#DC2626")
                        End With
                        r += 2

                        ' ── COBROS ──────────────────────────────────────────────────────
                        ws.Range(r, 1, r, totalCols).Merge()
                        ws.Cell(r, 1).Value = "COBROS"
                        ws.Range(r, 1, r, totalCols).Style.Fill.BackgroundColor = XLColor.FromHtml("#16A34A")
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True
                            .FontSize = 11
                            .FontColor = XLColor.White
                        End With
                        r += 1

                        ws.Cell(r, 1).Value = "Monto Cobrado"
                        ws.Range(r, 1, r, totalCols).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0FDF4")
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True
                            .FontColor = XLColor.FromHtml("#15803D")
                        End With
                        r += 1

                        Dim totalCobros As Decimal = 0
                        For Each c In cobros
                            totalCobros += c.Cantidad
                            ws.Cell(r, 1).Value = c.Cantidad.ToString("N2") & " L"
                            r += 1
                        Next
                        If cobros.Count = 0 Then
                            ws.Cell(r, 1).Value = "(Sin cobros registrados)"
                            With ws.Cell(r, 1).Style.Font
                                .Italic = True
                                .FontColor = XLColor.FromHtml("#94A3B8")
                            End With
                            ws.Range(r, 1, r, totalCols).Merge()
                            r += 1
                        End If

                        Dim ctotalRange = ws.Range(r, 1, r, totalCols)
                        ctotalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC")
                        ctotalRange.Style.Border.TopBorder = XLBorderStyleValues.Medium
                        ctotalRange.Style.Border.TopBorderColor = XLColor.FromHtml("#16A34A")
                        ws.Cell(r, 1).Value = "TOTAL COBROS:"
                        ws.Cell(r, 1).Style.Font.Bold = True
                        ws.Cell(r, 2).Value = totalCobros.ToString("N2") & " L"
                        With ws.Cell(r, 2).Style.Font
                            .Bold = True
                            .FontColor = XLColor.FromHtml("#16A34A")
                        End With
                        r += 2

                        ' ── BALANCE ─────────────────────────────────────────────────────
                        Dim balance = totalCobros - totalGastos
                        Dim balColor = If(balance >= 0, XLColor.FromHtml("#16A34A"), XLColor.FromHtml("#DC2626"))
                        Dim balRange = ws.Range(r, 1, r, totalCols)
                        balRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#EFF6FF")
                        balRange.Style.Border.TopBorder = XLBorderStyleValues.Double
                        balRange.Style.Border.TopBorderColor = XLColor.FromHtml("#1E40AF")

                        ws.Cell(r, 1).Value = "BALANCE  (Cobros - Gastos):"
                        With ws.Cell(r, 1).Style.Font
                            .Bold = True
                            .FontSize = 11
                            .FontColor = XLColor.FromHtml("#1E40AF")
                        End With
                        ws.Range(r, 1, r, 3).Merge()

                        ws.Cell(r, 4).Value = balance.ToString("N2") & " L"
                        With ws.Cell(r, 4).Style.Font
                            .Bold = True
                            .FontSize = 11
                            .FontColor = balColor
                        End With

                        ws.Cell(r, 5).Value = If(balance >= 0, "SUPERÁVIT", "DÉFICIT")
                        With ws.Cell(r, 5).Style.Font
                            .Bold = True
                            .FontColor = balColor
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

        ' ─── Construye la lista plana de filas financieras ───────────────────
        Private Function BuildRows(gastos As List(Of GastoRemedida), cobros As List(Of CobroRemedida)) As List(Of FinancialRow)
            Dim rows As New List(Of FinancialRow)()

            rows.Add(New FinancialRow With {.Tipo = "SECTION_G", .Col1 = "GASTOS", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            rows.Add(New FinancialRow With {.Tipo = "HDR_G", .Col1 = "Descripción", .Col2 = "Cantidad", .Col3 = "Costo Unit.", .Col4 = "Total", .Col5 = "Estado"})

            Dim totalGastos As Decimal = 0
            For Each g In gastos
                Dim total = g.Cantidad * g.CostoUnitario
                totalGastos += total
                rows.Add(New FinancialRow With {
                    .Tipo = "GASTO",
                    .Col1 = g.Descripcion,
                    .Col2 = g.Cantidad.ToString("N2"),
                    .Col3 = g.CostoUnitario.ToString("N2"),
                    .Col4 = total.ToString("N2"),
                    .Col5 = If(g.PendienteDePago, "Pendiente", "Pagado")
                })
            Next
            If gastos.Count = 0 Then
                rows.Add(New FinancialRow With {.Tipo = "EMPTY", .Col1 = "(Sin gastos registrados)", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            End If
            rows.Add(New FinancialRow With {.Tipo = "TOTAL_G", .Col1 = "", .Col2 = "", .Col3 = "TOTAL GASTOS:", .Col4 = totalGastos.ToString("N2") & " L", .Col5 = ""})

            rows.Add(New FinancialRow With {.Tipo = "SPACER", .Col1 = "", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})

            rows.Add(New FinancialRow With {.Tipo = "SECTION_C", .Col1 = "COBROS", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            rows.Add(New FinancialRow With {.Tipo = "HDR_C", .Col1 = "Monto Cobrado", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})

            Dim totalCobros As Decimal = 0
            For Each c In cobros
                totalCobros += c.Cantidad
                rows.Add(New FinancialRow With {
                    .Tipo = "COBRO",
                    .Col1 = c.Cantidad.ToString("N2") & " L",
                    .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""
                })
            Next
            If cobros.Count = 0 Then
                rows.Add(New FinancialRow With {.Tipo = "EMPTY", .Col1 = "(Sin cobros registrados)", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})
            End If
            rows.Add(New FinancialRow With {.Tipo = "TOTAL_C", .Col1 = "TOTAL COBROS:", .Col2 = totalCobros.ToString("N2") & " L", .Col3 = "", .Col4 = "", .Col5 = ""})

            rows.Add(New FinancialRow With {.Tipo = "SPACER", .Col1 = "", .Col2 = "", .Col3 = "", .Col4 = "", .Col5 = ""})

            Dim balance = totalCobros - totalGastos
            rows.Add(New FinancialRow With {
                .Tipo = "BALANCE",
                .Col1 = "BALANCE  (Cobros - Gastos)",
                .Col2 = "",
                .Col3 = "",
                .Col4 = balance.ToString("N2") & " L",
                .Col5 = If(balance >= 0, "SUPERÁVIT", "DÉFICIT")
            })

            Return rows
        End Function

        Private Function BuildDataTable(rows As List(Of FinancialRow)) As DataTable
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
        Private Function BuildRdlcXml(remedida As Remedida) As String
            Const pageW As Double = 8.5
            Const pageH As Double = 11.0
            Const margin As Double = 0.5
            Const cW As Double = 7.5              ' contentWidth = pageW - 2*margin
            Const logoSize As Double = 0.9
            Const logoRight As Double = 1.05      ' info text starts here
            Const cWInfo As Double = 6.45         ' cW - logoRight
            Const sep1Top As Double = 0.97

            ' Remedida info block
            Const ibTop As Double = 1.08           ' infoBlockTop
            Const ibRowH As Double = 0.22
            Const ibLeftW As Double = 3.5          ' left column width
            Const ibRightX As Double = 3.9         ' right column x
            Const ibRightW As Double = 3.6         ' cW - ibRightX
            Const ibRows As Integer = 4            ' rows in left/right columns

            Const objetoTop As Double = ibTop + ibRows * ibRowH + 0.02
            Const sep2Top As Double = objetoTop + ibRowH + 0.04
            Const tablixTop As Double = sep2Top + 0.12

            Dim fechaGen = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim logoB64 = GetLogoBase64()
            Dim hasLogo = Not String.IsNullOrEmpty(logoB64)

            ' ── Expresiones condicionales para el Tablix ──
            Dim bgExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"",""#EA580C"",IIF(Fields!Tipo.Value=""SECTION_C"",""#16A34A"",IIF(Fields!Tipo.Value=""HDR_G"",""#FFF7ED"",IIF(Fields!Tipo.Value=""HDR_C"",""#F0FDF4"",IIF(Fields!Tipo.Value=""TOTAL_G"" OR Fields!Tipo.Value=""TOTAL_C"",""#F8FAFC"",IIF(Fields!Tipo.Value=""BALANCE"",""#EFF6FF"",""White""))))))"
            Dim colorExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_C"",""White"",IIF(Fields!Tipo.Value=""HDR_G"",""#C2410C"",IIF(Fields!Tipo.Value=""HDR_C"",""#15803D"",IIF(Fields!Tipo.Value=""BALANCE"",""#1E40AF"",""#1F2937""))))"
            Dim boldExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_C"" OR Fields!Tipo.Value=""HDR_G"" OR Fields!Tipo.Value=""HDR_C"" OR Fields!Tipo.Value=""TOTAL_G"" OR Fields!Tipo.Value=""TOTAL_C"" OR Fields!Tipo.Value=""BALANCE"",""Bold"",""Normal"")"
            Dim sizeExpr = "=IIF(Fields!Tipo.Value=""SECTION_G"" OR Fields!Tipo.Value=""SECTION_C"" OR Fields!Tipo.Value=""BALANCE"",""10pt"",""8pt"")"

            Dim sb As New StringBuilder()
            sb.Append("<?xml version=""1.0"" encoding=""utf-8""?>")
            sb.Append("<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" ")
            sb.Append("xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">")

            ' EmbeddedImages
            If hasLogo Then
                sb.Append("<EmbeddedImages><EmbeddedImage Name=""VemarLogo"">")
                sb.Append("<MIMEType>image/png</MIMEType>")
                sb.Append($"<ImageData>{logoB64}</ImageData>")
                sb.Append("</EmbeddedImage></EmbeddedImages>")
            End If

            ' DataSources + DataSets
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

            ' Body
            sb.Append("<Body><ReportItems>")

            ' Logo
            If hasLogo Then
                sb.Append("<Image Name=""ImgLogo""><Source>Embedded</Source><Value>VemarLogo</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append("<Top>0in</Top><Left>0in</Left>")
                sb.Append($"<Height>{FmtIn(logoSize)}</Height><Width>{FmtIn(logoSize)}</Width>")
                sb.Append("<Style/></Image>")
            End If

            Dim hLeft = FmtIn(If(hasLogo, logoRight, 0.0))
            Dim hWidth = FmtIn(If(hasLogo, cWInfo, cW))

            ' Encabezado empresa / título / fecha
            AppendTxt(sb, "TxtEmpresa", "CONSTRUCTORA VEMAR S. de R.L. de C.V.", "0.05in", hLeft, "0.28in", hWidth, "13pt", "Bold", "#1E3A8A")
            AppendTxt(sb, "TxtTitulo", "REPORTE FINANCIERO DE REMEDIDA", "0.35in", hLeft, "0.25in", hWidth, "11pt", "Bold", "#1E40AF")
            AppendTxt(sb, "TxtFecha", $"Generado: {fechaGen}", "0.62in", hLeft, "0.18in", hWidth, "8pt", "Normal", "#64748B")

            AppendSep(sb, "RctSep1", FmtIn(sep1Top), FmtIn(cW), "#1E40AF")

            ' Datos encabezado remedida (4 filas x 2 columnas)
            Dim leftData = New (String, String)() {
                ("Representante:", remedida.Representante),
                ("Ubicación:", remedida.Ubicacion),
                ("Clave SURE:", remedida.ClaveSure),
                ("CAM:", remedida.Cam)
            }
            Dim rightData = New (String, String)() {
                ("Fecha:", remedida.Fecha.ToString("dd/MM/yyyy")),
                ("Precio Pactado:", remedida.Precio.ToString("N2") & " L"),
                ("Matrícula:", remedida.Matricula),
                ("Expediente:", If(remedida.ExpedienteEntregado, "Entregado", "Pendiente"))
            }
            For i = 0 To ibRows - 1
                Dim topFmt = FmtIn(ibTop + i * ibRowH)
                AppendInfoTxt(sb, $"InfoL{i}", leftData(i).Item1, leftData(i).Item2, topFmt, "0in", FmtIn(ibRowH), FmtIn(ibLeftW))
                AppendInfoTxt(sb, $"InfoR{i}", rightData(i).Item1, rightData(i).Item2, topFmt, FmtIn(ibRightX), FmtIn(ibRowH), FmtIn(ibRightW))
            Next

            ' Objeto (ancho completo)
            AppendInfoTxt(sb, "InfoObjeto", "Objeto:", remedida.Objeto, FmtIn(objetoTop), "0in", FmtIn(ibRowH), FmtIn(cW))

            AppendSep(sb, "RctSep2", FmtIn(sep2Top), FmtIn(cW), "#94A3B8")

            ' Tablix financiero
            Dim colWidths = {3.0, 0.8, 1.0, 1.2, 1.5}
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

        ' ─── Helpers de generación RDLC ────────────────────────────────────
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
