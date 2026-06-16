Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class MovimientosRemedidaReport : Inherits RdlcReportBase(Of Movimiento)

        Private ReadOnly _remedidaNombre As String

        Public Sub New(remedidaNombre As String)
            _remedidaNombre = remedidaNombre
        End Sub

        Protected Overrides ReadOnly Property ReportTitle As String
            Get
                Return $"Movimientos — {_remedidaNombre}"
            End Get
        End Property

        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Tipo Movimiento", "TipoMovimiento", "2in"),
                    New ReportColumn("Fecha", "Fecha", "1in"),
                    New ReportColumn("Descripción", "Descripcion", "6.5in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Movimiento) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"TipoMovimiento", If(item.TipoMovimiento?.Tipo, "")},
                {"Fecha", item.Fecha.ToString("dd/MM/yyyy")},
                {"Descripcion", If(item.Descripcion, "")}
            }
        End Function
    End Class
End Namespace
