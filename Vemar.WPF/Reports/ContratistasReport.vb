Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ContratistasReport : Inherits RdlcReportBase(Of Contratista)
        Protected Overrides ReadOnly Property ReportTitle As String = "Contratistas"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = False

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.8in"),
                    New ReportColumn("Nombre", "Nombre", "4in"),
                    New ReportColumn("Teléfono", "Telefono", "2.7in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Contratista) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Nombre", item.Nombre},
                {"Telefono", item.Telefono}
            }
        End Function
    End Class
End Namespace
