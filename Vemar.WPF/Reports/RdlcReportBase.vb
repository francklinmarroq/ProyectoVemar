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

Namespace Vemar.WPF.Reports

    Public Structure ReportColumn
        Public Property Header As String
        Public Property FieldName As String
        Public Property Width As String

        Public Sub New(header As String, fieldName As String, Optional width As String = "1.5in")
            Me.Header = header
            Me.FieldName = fieldName
            Me.Width = width
        End Sub
    End Structure

    Public MustInherit Class RdlcReportBase(Of T)
        Protected MustOverride ReadOnly Property ReportTitle As String
        Protected MustOverride ReadOnly Property Columns As List(Of ReportColumn)
        Protected MustOverride Function GetRowValues(item As T) As Dictionary(Of String, String)
        Protected Overridable ReadOnly Property IsLandscape As Boolean = True

        ' Logo embebido como recurso del ensamblado
        Private Shared _logoBase64 As String = Nothing

        Private Shared Function GetLogoBase64() As String
            If _logoBase64 IsNot Nothing Then Return _logoBase64
            Try
                Dim asm = Assembly.GetExecutingAssembly()
                Dim resourceName = "Vemar.WPF.logo.png"
                Using stream = asm.GetManifestResourceStream(resourceName)
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

        ' ─── PDF via RDLC ───────────────────────────────────────────────
        Public Async Function GeneratePdfAsync(data As List(Of T), fileName As String) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim dt = BuildDataTable(data)
                    Dim rdlcXml = BuildRdlcXml(data.Count)

                    Dim report As New LocalReport()
                    Using stream As New MemoryStream(Encoding.UTF8.GetBytes(rdlcXml))
                        report.LoadReportDefinition(stream)
                    End Using
                    report.DataSources.Add(New ReportDataSource("DataSet1", dt))

                    Dim pdfBytes = report.Render("PDF")

                    PdfPreviewHelper.ShowPreview(pdfBytes, ReportTitle, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}")
                    Return String.Empty
                Catch ex As Exception
                    Dim msg = ex.Message
                    Dim inner = ex.InnerException
                    Do While inner IsNot Nothing
                        msg &= " → " & inner.Message
                        inner = inner.InnerException
                    Loop
                    Application.Current.Dispatcher.Invoke(Sub()
                        MessageBox.Show("Error al generar PDF: " & msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Sub)
                    Return String.Empty
                End Try
            End Function)
        End Function

        ' ─── Excel via ClosedXML ─────────────────────────────────────────
        Public Async Function GenerateExcelAsync(data As List(Of T), fileName As String) As Task(Of String)
            Return Await Task.Run(Function()
                Try
                    Dim desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    Dim filePath = Path.Combine(desktopPath, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx")

                    Using wb As New XLWorkbook()
                        Dim ws = wb.Worksheets.Add(ReportTitle)

                        ' ── Logo (esquina superior derecha) ──
                        Try
                            Dim asm = Assembly.GetExecutingAssembly()
                            Using imgStream = asm.GetManifestResourceStream("Vemar.WPF.logo.png")
                                If imgStream IsNot Nothing Then
                                    Dim bmp As New System.Drawing.Bitmap(imgStream)
                                    Dim pic = ws.AddPicture(bmp)
                                    pic.MoveTo(ws.Cell(1, Columns.Count))
                                    pic.Width = 70
                                    pic.Height = 70
                                End If
                            End Using
                        Catch
                            ' Si el logo no está disponible se omite sin error
                        End Try

                        ' ── Encabezado empresa ──
                        ws.Cell(1, 1).Value = "CONSTRUCTORA VEMAR S. de R.L. de C.V."
                        ws.Cell(1, 1).Style.Font.Bold = True
                        ws.Cell(1, 1).Style.Font.FontSize = 14
                        ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1E3A8A")
                        ws.Range(1, 1, 1, Columns.Count).Merge()

                        ' ── Título del reporte ──
                        ws.Cell(2, 1).Value = "REPORTE DE " & ReportTitle.ToUpper()
                        ws.Cell(2, 1).Style.Font.Bold = True
                        ws.Cell(2, 1).Style.Font.FontSize = 12
                        ws.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#1E40AF")
                        ws.Range(2, 1, 2, Columns.Count).Merge()

                        ' ── Fecha y total ──
                        ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}     Total de registros: {data.Count}"
                        ws.Cell(3, 1).Style.Font.Italic = True
                        ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromHtml("#64748B")
                        ws.Cell(3, 1).Style.Font.FontSize = 10
                        ws.Range(3, 1, 3, Columns.Count).Merge()

                        ' ── Línea separadora (fila vacía con borde inferior) ──
                        Dim sepRange = ws.Range(4, 1, 4, Columns.Count)
                        sepRange.Merge()
                        sepRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium
                        sepRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#1E40AF")

                        ' ── Encabezados de columna ──
                        For i = 0 To Columns.Count - 1
                            Dim cell = ws.Cell(5, i + 1)
                            cell.Value = Columns(i).Header
                            cell.Style.Font.Bold = True
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E40AF")
                            cell.Style.Font.FontColor = XLColor.White
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                            cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium
                            cell.Style.Border.BottomBorderColor = XLColor.FromHtml("#3B82F6")
                        Next

                        ' ── Datos ──
                        For row = 0 To data.Count - 1
                            Dim values = GetRowValues(data(row))
                            For i = 0 To Columns.Count - 1
                                Dim fn = Columns(i).FieldName
                                Dim v = If(values.ContainsKey(fn), values(fn), "")
                                Dim c = ws.Cell(row + 6, i + 1)
                                c.Value = v
                                If row Mod 2 = 1 Then
                                    c.Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F9FF")
                                End If
                            Next
                        Next

                        ' ── Sección de firmas ──
                        Dim lastDataRow = data.Count + 6   ' 5 header rows + data + 1-based
                        Dim sigRow1 = lastDataRow + 2      ' fila línea firma
                        Dim sigRow2 = lastDataRow + 3      ' fila etiqueta firma
                        Dim midCol = Math.Max(2, Columns.Count \ 2)
                        Dim rightCol = Columns.Count

                        ' Línea Entrega
                        ws.Cell(sigRow1, 1).Value = New String("_"c, 28)
                        ws.Cell(sigRow1, 1).Style.Font.FontColor = XLColor.FromHtml("#374151")
                        ws.Range(sigRow1, 1, sigRow1, midCol - 1).Merge()

                        ' Línea Recibe
                        ws.Cell(sigRow1, midCol + 1).Value = New String("_"c, 28)
                        ws.Cell(sigRow1, midCol + 1).Style.Font.FontColor = XLColor.FromHtml("#374151")
                        ws.Range(sigRow1, midCol + 1, sigRow1, rightCol).Merge()

                        ' Etiqueta Entrega
                        ws.Cell(sigRow2, 1).Value = "Entrega"
                        ws.Cell(sigRow2, 1).Style.Font.Bold = True
                        ws.Cell(sigRow2, 1).Style.Font.FontSize = 10
                        ws.Cell(sigRow2, 1).Style.Font.FontColor = XLColor.FromHtml("#374151")
                        ws.Cell(sigRow2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                        ws.Range(sigRow2, 1, sigRow2, midCol - 1).Merge()

                        ' Etiqueta Recibe
                        ws.Cell(sigRow2, midCol + 1).Value = "Recibe"
                        ws.Cell(sigRow2, midCol + 1).Style.Font.Bold = True
                        ws.Cell(sigRow2, midCol + 1).Style.Font.FontSize = 10
                        ws.Cell(sigRow2, midCol + 1).Style.Font.FontColor = XLColor.FromHtml("#374151")
                        ws.Cell(sigRow2, midCol + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                        ws.Range(sigRow2, midCol + 1, sigRow2, rightCol).Merge()

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

        ' ─── Construye DataTable ─────────────────────────────────────────
        Private Function BuildDataTable(data As List(Of T)) As DataTable
            Dim dt As New DataTable("DataSet1")
            For Each col In Columns
                dt.Columns.Add(col.FieldName, GetType(String))
            Next
            For Each item In data
                Dim values = GetRowValues(item)
                Dim row = dt.NewRow()
                For Each col In Columns
                    row(col.FieldName) = If(values.ContainsKey(col.FieldName) AndAlso values(col.FieldName) IsNot Nothing,
                                            values(col.FieldName), "")
                Next
                dt.Rows.Add(row)
            Next
            Return dt
        End Function

        ' ─── Genera RDLC XML dinámicamente ───────────────────────────────
        Private Function BuildRdlcXml(totalRecords As Integer) As String
            Dim cols = Columns
            Dim totalIn As Double = cols.Sum(Function(c) ParseIn(c.Width))
            Dim pageW As Double = If(IsLandscape, 11.0, 8.5)
            Dim pageH As Double = If(IsLandscape, 8.5, 11.0)
            Const margin As Double = 0.5
            Dim avail As Double = pageW - margin * 2

            ' Escalar columnas si no caben
            If totalIn > avail Then
                Dim scale = avail / totalIn
                cols = cols.Select(Function(c) New ReportColumn(c.Header, c.FieldName, FmtIn(ParseIn(c.Width) * scale))).ToList()
                totalIn = avail
            End If

            Dim totalW = FmtIn(totalIn)
            Dim fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            Dim bannerB64 = ReportHeaderHelper.GetBannerBase64()
            Dim hasBanner = Not String.IsNullOrEmpty(bannerB64)

            ' Dimensiones del encabezado
            Const bannerH As Double = 1.60     ' altura del banner
            Const sepTop As Double = 1.64      ' línea separadora azul
            Const tablixTop As Double = 1.88   ' inicio del tablix (deja espacio para título)

            Dim sb As New StringBuilder()

            sb.Append("<?xml version=""1.0"" encoding=""utf-8""?>")
            sb.Append("<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" ")
            sb.Append("xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">")

            ' ── EmbeddedImages (banner) ────────────────────────────────
            If hasBanner Then
                sb.Append("<EmbeddedImages>")
                sb.Append("<EmbeddedImage Name=""VemarBanner"">")
                sb.Append("<MIMEType>image/jpeg</MIMEType>")
                sb.Append($"<ImageData>{bannerB64}</ImageData>")
                sb.Append("</EmbeddedImage>")
                sb.Append("</EmbeddedImages>")
            End If

            ' ── DataSources ───────────────────────────────────────────
            sb.Append("<DataSources>")
            sb.Append("<DataSource Name=""DataSource1"">")
            sb.Append("<ConnectionProperties>")
            sb.Append("<DataProvider>System.Data.DataSet</DataProvider>")
            sb.Append("<ConnectString>/* Local Connection */</ConnectString>")
            sb.Append("</ConnectionProperties>")
            sb.Append("</DataSource>")
            sb.Append("</DataSources>")

            ' ── DataSets ──────────────────────────────────────────────
            sb.Append("<DataSets>")
            sb.Append("<DataSet Name=""DataSet1"">")
            sb.Append("<Query>")
            sb.Append("<DataSourceName>DataSource1</DataSourceName>")
            sb.Append("<CommandText>/* Local Query */</CommandText>")
            sb.Append("</Query>")
            sb.Append("<Fields>")
            For Each col In cols
                sb.Append($"<Field Name=""{col.FieldName}"">")
                sb.Append($"<DataField>{col.FieldName}</DataField>")
                sb.Append("</Field>")
            Next
            sb.Append("</Fields>")
            sb.Append("</DataSet>")
            sb.Append("</DataSets>")

            ' ── Body ──────────────────────────────────────────────────
            sb.Append("<Body>")
            sb.Append("<ReportItems>")

            ' ── Banner corporativo ────────────────────────────────────────
            If hasBanner Then
                Dim bannerW As Double = 5.12
                Dim bannerLeft As Double = Math.Max(0, (totalIn - bannerW) / 2.0)
                sb.Append("<Image Name=""ImgBanner"">")
                sb.Append("<Source>Embedded</Source><Value>VemarBanner</Value>")
                sb.Append("<Sizing>FitProportional</Sizing>")
                sb.Append($"<Top>0in</Top><Left>{FmtIn(bannerLeft)}</Left><Height>{FmtIn(bannerH)}</Height><Width>{FmtIn(bannerW)}</Width>")
                sb.Append("<Style/>")
                sb.Append("</Image>")
            End If

            ' ── Línea azul separadora ─────────────────────────────────────
            sb.Append("<Rectangle Name=""RctSep"">")
            sb.Append($"<Top>{FmtIn(sepTop)}</Top><Left>0in</Left><Height>0.04in</Height><Width>{totalW}</Width>")
            sb.Append("<Style><BackgroundColor>#1E3A8A</BackgroundColor></Style>")
            sb.Append("</Rectangle>")

            ' ── Título del reporte debajo de la línea ─────────────────────
            sb.Append("<Textbox Name=""TxtTitulo"">")
            sb.Append("<CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<Style><TextAlign>Left</TextAlign></Style>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append($"<Value>REPORTE DE {XmlEsc(ReportTitle.ToUpper())}   —   {totalRecords} registro(s)</Value>")
            sb.Append("<Style><FontSize>9pt</FontSize><FontWeight>Bold</FontWeight><Color>#1E3A8A</Color></Style>")
            sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
            sb.Append($"<Top>{FmtIn(sepTop + 0.07)}</Top><Left>0in</Left><Height>0.18in</Height><Width>{totalW}</Width>")
            sb.Append("</Textbox>")

            ' ── Tablix ────────────────────────────────────────────────
            sb.Append($"<Tablix Name=""Tablix1"">")
            sb.Append("<TablixBody>")

            sb.Append("<TablixColumns>")
            For Each col In cols
                sb.Append($"<TablixColumn><Width>{col.Width}</Width></TablixColumn>")
            Next
            sb.Append("</TablixColumns>")

            sb.Append("<TablixRows>")

            ' Fila de encabezados
            sb.Append("<TablixRow><Height>0.28in</Height><TablixCells>")
            For i = 0 To cols.Count - 1
                Dim col = cols(i)
                sb.Append("<TablixCell><CellContents>")
                sb.Append($"<Textbox Name=""Hdr{i}"">")
                sb.Append("<CanGrow>true</CanGrow>")
                sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
                sb.Append($"<Value>{XmlEsc(col.Header)}</Value>")
                sb.Append("<Style><FontWeight>Bold</FontWeight><Color>White</Color><FontSize>8pt</FontSize></Style>")
                sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
                sb.Append("<Style>")
                sb.Append("<BackgroundColor>#1E40AF</BackgroundColor>")
                sb.Append("<PaddingLeft>4pt</PaddingLeft>")
                sb.Append("<PaddingRight>4pt</PaddingRight>")
                sb.Append("<PaddingTop>2pt</PaddingTop>")
                sb.Append("<PaddingBottom>2pt</PaddingBottom>")
                sb.Append("</Style>")
                sb.Append("</Textbox>")
                sb.Append("</CellContents></TablixCell>")
            Next
            sb.Append("</TablixCells></TablixRow>")

            ' Fila de datos
            sb.Append("<TablixRow><Height>0.22in</Height><TablixCells>")
            For i = 0 To cols.Count - 1
                Dim col = cols(i)
                sb.Append("<TablixCell><CellContents>")
                sb.Append($"<Textbox Name=""Val{i}"">")
                sb.Append("<CanGrow>true</CanGrow>")
                sb.Append("<Paragraphs><Paragraph><TextRuns><TextRun>")
                sb.Append($"<Value>=Fields!{col.FieldName}.Value</Value>")
                sb.Append("<Style><FontSize>8pt</FontSize><Color>#1F2937</Color></Style>")
                sb.Append("</TextRun></TextRuns></Paragraph></Paragraphs>")
                sb.Append("<Style>")
                sb.Append("<BottomBorder><Color>#E2E8F0</Color><Style>Solid</Style><Width>0.5pt</Width></BottomBorder>")
                sb.Append("<PaddingLeft>4pt</PaddingLeft>")
                sb.Append("<PaddingRight>4pt</PaddingRight>")
                sb.Append("<PaddingTop>2pt</PaddingTop>")
                sb.Append("<PaddingBottom>2pt</PaddingBottom>")
                sb.Append("</Style>")
                sb.Append("</Textbox>")
                sb.Append("</CellContents></TablixCell>")
            Next
            sb.Append("</TablixCells></TablixRow>")

            sb.Append("</TablixRows>")
            sb.Append("</TablixBody>")

            ' Jerarquía de columnas
            sb.Append("<TablixColumnHierarchy><TablixMembers>")
            For Each col In cols
                sb.Append("<TablixMember/>")
            Next
            sb.Append("</TablixMembers></TablixColumnHierarchy>")

            ' Jerarquía de filas
            sb.Append("<TablixRowHierarchy><TablixMembers>")
            sb.Append("<TablixMember>")
            sb.Append("<KeepWithGroup>After</KeepWithGroup>")
            sb.Append("<RepeatOnNewPage>true</RepeatOnNewPage>")
            sb.Append("</TablixMember>")
            sb.Append("<TablixMember><Group Name=""Details""/></TablixMember>")
            sb.Append("</TablixMembers></TablixRowHierarchy>")

            sb.Append("<DataSetName>DataSet1</DataSetName>")
            sb.Append($"<Top>{FmtIn(tablixTop)}</Top>")
            sb.Append("<Left>0in</Left>")
            sb.Append("<Height>0.5in</Height>")
            sb.Append($"<Width>{totalW}</Width>")
            sb.Append("</Tablix>")

            sb.Append("</ReportItems>")

            ' Altura del body basada en el contenido real (no la página completa),
            ' para que reportes con pocas filas no generen una segunda página vacía.
            Const headerRowH As Double = 0.28
            Const dataRowH As Double = 0.22
            Dim contentH As Double = tablixTop + headerRowH + (totalRecords * dataRowH) + 0.10
            Dim maxBodyH As Double = pageH - 0.5 - 0.75 - 0.02   ' topMargin + bottomMargin - epsilon
            Dim bodyH As Double = Math.Min(contentH, maxBodyH)
            sb.Append($"<Height>{FmtIn(bodyH)}</Height>")
            sb.Append("</Body>")

            ' Width del reporte (requerido en schema 2008/01)
            sb.Append($"<Width>{totalW}</Width>")

            ' Página
            sb.Append("<Page>")

            ' ── Pie de página con firmas ────────────────────────────────
            Const footerH As Double = 0.55
            Dim sigLineW As Double = 2.3
            Dim sigEntregaLeft As Double = avail / 4.0 - sigLineW / 2.0
            Dim sigRecibeLeft As Double = avail * 3.0 / 4.0 - sigLineW / 2.0

            sb.Append("<PageFooter>")
            sb.Append("<PrintOnFirstPage>true</PrintOnFirstPage>")
            sb.Append("<PrintOnLastPage>true</PrintOnLastPage>")
            sb.Append("<ReportItems>")

            ' Línea "Entrega"
            sb.Append("<Rectangle Name=""RctFirmaEntrega"">")
            sb.Append($"<Top>0.08in</Top><Left>{FmtIn(sigEntregaLeft)}</Left><Height>0.02in</Height><Width>{FmtIn(sigLineW)}</Width>")
            sb.Append("<Style><BottomBorder><Color>#374151</Color><Style>Solid</Style><Width>1pt</Width></BottomBorder></Style>")
            sb.Append("</Rectangle>")

            ' Etiqueta "Entrega"
            sb.Append("<Textbox Name=""TxtFirmaEntrega"">")
            sb.Append("<CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append("<Value>Entrega</Value>")
            sb.Append("<Style><FontSize>8pt</FontSize><FontWeight>Bold</FontWeight><Color>#374151</Color></Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append("<Style><TextAlign>Center</TextAlign></Style>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>0.12in</Top><Left>{FmtIn(sigEntregaLeft)}</Left><Height>0.18in</Height><Width>{FmtIn(sigLineW)}</Width>")
            sb.Append("</Textbox>")

            ' Línea "Recibe"
            sb.Append("<Rectangle Name=""RctFirmaRecibe"">")
            sb.Append($"<Top>0.08in</Top><Left>{FmtIn(sigRecibeLeft)}</Left><Height>0.02in</Height><Width>{FmtIn(sigLineW)}</Width>")
            sb.Append("<Style><BottomBorder><Color>#374151</Color><Style>Solid</Style><Width>1pt</Width></BottomBorder></Style>")
            sb.Append("</Rectangle>")

            ' Etiqueta "Recibe"
            sb.Append("<Textbox Name=""TxtFirmaRecibe"">")
            sb.Append("<CanGrow>true</CanGrow>")
            sb.Append("<Paragraphs><Paragraph>")
            sb.Append("<TextRuns><TextRun>")
            sb.Append("<Value>Recibe</Value>")
            sb.Append("<Style><FontSize>8pt</FontSize><FontWeight>Bold</FontWeight><Color>#374151</Color></Style>")
            sb.Append("</TextRun></TextRuns>")
            sb.Append("<Style><TextAlign>Center</TextAlign></Style>")
            sb.Append("</Paragraph></Paragraphs>")
            sb.Append($"<Top>0.12in</Top><Left>{FmtIn(sigRecibeLeft)}</Left><Height>0.18in</Height><Width>{FmtIn(sigLineW)}</Width>")
            sb.Append("</Textbox>")

            sb.Append("</ReportItems>")
            sb.Append($"<Height>{FmtIn(footerH)}</Height>")
            sb.Append("</PageFooter>")

            sb.Append($"<PageHeight>{FmtIn(pageH)}</PageHeight>")
            sb.Append($"<PageWidth>{FmtIn(pageW)}</PageWidth>")
            sb.Append($"<LeftMargin>{FmtIn(margin)}</LeftMargin>")
            sb.Append($"<RightMargin>{FmtIn(margin)}</RightMargin>")
            sb.Append("<TopMargin>0.50in</TopMargin>")
            sb.Append("<BottomMargin>0.75in</BottomMargin>")
            sb.Append("</Page>")

            sb.Append("<Language>es-HN</Language>")
            sb.Append("</Report>")

            Return sb.ToString()
        End Function

        Private Function ParseIn(w As String) As Double
            Return Double.Parse(w.Replace("in", ""), CultureInfo.InvariantCulture)
        End Function

        Private Function FmtIn(d As Double) As String
            Return d.ToString("F2", CultureInfo.InvariantCulture) & "in"
        End Function

        Private Function XmlEsc(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
        End Function
    End Class

End Namespace
