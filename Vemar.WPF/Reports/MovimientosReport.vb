Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class MovimientosReport : Inherits RdlcReportBase(Of Movimiento)
        Protected Overrides ReadOnly Property ReportTitle As String = "Movimientos"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Remedida", "Remedida", "2.2in"),
                    New ReportColumn("Tipo Movimiento", "TipoMovimiento", "1.8in"),
                    New ReportColumn("Fecha", "Fecha", "0.9in"),
                    New ReportColumn("Descripción", "Descripcion", "4.6in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Movimiento) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Remedida", item.Remedida?.Representante},
                {"TipoMovimiento", item.TipoMovimiento?.Tipo},
                {"Fecha", item.Fecha.ToString("dd/MM/yyyy")},
                {"Descripcion", item.Descripcion}
            }
        End Function
    End Class
End Namespace
