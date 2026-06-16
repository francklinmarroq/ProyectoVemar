Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ClientesReport : Inherits RdlcReportBase(Of Cliente)
        Protected Overrides ReadOnly Property ReportTitle As String = "Clientes"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Nombre", "Nombre", "2in"),
                    New ReportColumn("RTN", "Rtn", "1.2in"),
                    New ReportColumn("Teléfono", "Telefono", "1.1in"),
                    New ReportColumn("Email", "Email", "2in"),
                    New ReportColumn("Representante", "Representante", "1.8in"),
                    New ReportColumn("Dirección", "Direccion", "1.9in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Cliente) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Nombre", item.Nombre},
                {"Rtn", item.Rtn},
                {"Telefono", item.Telefono},
                {"Email", item.EmailCorporativo},
                {"Representante", item.Representante},
                {"Direccion", item.Direccion}
            }
        End Function
    End Class
End Namespace
