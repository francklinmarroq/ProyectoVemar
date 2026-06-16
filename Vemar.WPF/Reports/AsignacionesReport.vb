Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class AsignacionesReport : Inherits RdlcReportBase(Of Asignacion)
        Protected Overrides ReadOnly Property ReportTitle As String = "Asignaciones"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Colaborador", "Colaborador", "2in"),
                    New ReportColumn("Proyecto", "Proyecto", "2in"),
                    New ReportColumn("Cliente", "Cliente", "1.8in"),
                    New ReportColumn("F. Inicio", "FechaAsignacion", "0.9in"),
                    New ReportColumn("F. Fin", "FechaFinalizacion", "0.9in"),
                    New ReportColumn("Observaciones", "Observaciones", "2.4in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Asignacion) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Colaborador", item.Colaborador?.Nombre},
                {"Proyecto", item.Proyecto?.Nombre},
                {"Cliente", item.Cliente?.Nombre},
                {"FechaAsignacion", item.FechaAsignacion.ToString("dd/MM/yyyy")},
                {"FechaFinalizacion", item.FechaFinalizacion?.ToString("dd/MM/yyyy")},
                {"Observaciones", item.Observaciones}
            }
        End Function
    End Class
End Namespace
