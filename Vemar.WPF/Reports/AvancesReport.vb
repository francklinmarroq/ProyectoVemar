Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class AvancesReport : Inherits RdlcReportBase(Of Avance)
        Protected Overrides ReadOnly Property ReportTitle As String = "Avances de Proyectos"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Proyecto", "Proyecto", "2.5in"),
                    New ReportColumn("Descripción", "Descripcion", "5in"),
                    New ReportColumn("Fecha", "Fecha", "1in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Avance) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Proyecto", item.Proyecto?.Nombre},
                {"Descripcion", item.Descripcion},
                {"Fecha", item.Fecha?.ToString("dd/MM/yyyy")}
            }
        End Function
    End Class
End Namespace
