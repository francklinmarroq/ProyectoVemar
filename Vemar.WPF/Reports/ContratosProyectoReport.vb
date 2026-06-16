Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ContratosProyectoReport : Inherits RdlcReportBase(Of Contrato)

        Private ReadOnly _proyectoNombre As String

        Public Sub New(proyectoNombre As String)
            _proyectoNombre = proyectoNombre
        End Sub

        Protected Overrides ReadOnly Property ReportTitle As String
            Get
                Return $"Contratos — {_proyectoNombre}"
            End Get
        End Property

        Protected Overrides ReadOnly Property IsLandscape As Boolean = False

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.6in"),
                    New ReportColumn("Contratista", "Contratista", "2.5in"),
                    New ReportColumn("Valor", "Valor", "1.5in"),
                    New ReportColumn("Descripción", "Descripcion", "3.4in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Contrato) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Contratista", If(item.Contratista?.Nombre, "")},
                {"Valor", item.Valor.ToString("C2")},
                {"Descripcion", If(item.Descripcion, "")}
            }
        End Function
    End Class
End Namespace
