Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class RemedidasReport : Inherits RdlcReportBase(Of Remedida)
        Protected Overrides ReadOnly Property ReportTitle As String = "Remedidas"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Representante", "Representante", "2in"),
                    New ReportColumn("Ubicación", "Ubicacion", "1.8in"),
                    New ReportColumn("Clave SURE", "ClaveSure", "1.1in"),
                    New ReportColumn("Matrícula", "Matricula", "1.1in"),
                    New ReportColumn("Fecha", "Fecha", "0.9in"),
                    New ReportColumn("Precio", "Precio", "1in"),
                    New ReportColumn("Expediente", "Expediente", "0.9in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Remedida) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Representante", item.Representante},
                {"Ubicacion", item.Ubicacion},
                {"ClaveSure", item.ClaveSure},
                {"Matricula", item.Matricula},
                {"Fecha", item.Fecha.ToString("dd/MM/yyyy")},
                {"Precio", item.Precio.ToString("N2")},
                {"Expediente", If(item.ExpedienteEntregado, "Entregado", "No Entregado")}
            }
        End Function
    End Class
End Namespace
