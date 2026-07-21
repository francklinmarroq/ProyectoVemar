Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ContratosReport : Inherits RdlcReportBase(Of Contrato)
        Protected Overrides ReadOnly Property ReportTitle As String = "Contratos"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = False

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.6in"),
                    New ReportColumn("Contratista", "Contratista", "2in"),
                    New ReportColumn("Proyecto", "Proyecto", "2in"),
                    New ReportColumn("Valor", "Valor", "1.5in"),
                    New ReportColumn("Descripción", "Descripcion", "3.9in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Contrato) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Contratista", If(item.Contratista?.Nombre, "")},
                {"Proyecto", If(item.Proyecto?.Nombre, "")},
                {"Valor", "L. " & item.Valor.ToString("N2")},
                {"Descripcion", If(item.Descripcion, "")}
            }
        End Function
    End Class
End Namespace
