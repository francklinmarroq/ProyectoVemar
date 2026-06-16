Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class TramitesReport : Inherits RdlcReportBase(Of Tramite)
        Protected Overrides ReadOnly Property ReportTitle As String = "Trámites"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Tipo", "Tipo", "1.8in"),
                    New ReportColumn("Proyecto", "Proyecto", "2.2in"),
                    New ReportColumn("Estado", "Estado", "1.5in"),
                    New ReportColumn("Descripción", "Descripcion", "4in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Tramite) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Tipo", item.TipoTramite?.Nombre},
                {"Proyecto", item.Proyecto?.Nombre},
                {"Estado", item.EstadoTramite?.Estado},
                {"Descripcion", item.Descripcion}
            }
        End Function
    End Class
End Namespace
