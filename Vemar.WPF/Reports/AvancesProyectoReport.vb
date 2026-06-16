Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class AvancesProyectoReport : Inherits RdlcReportBase(Of Avance)

        Private ReadOnly _proyectoNombre As String

        Public Sub New(proyectoNombre As String)
            _proyectoNombre = proyectoNombre
        End Sub

        Protected Overrides ReadOnly Property ReportTitle As String
            Get
                Return $"Avances — {_proyectoNombre}"
            End Get
        End Property

        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Descripción", "Descripcion", "7.5in"),
                    New ReportColumn("Fecha", "Fecha", "1in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Avance) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Descripcion", If(item.Descripcion, "")},
                {"Fecha", If(item.Fecha?.ToString("dd/MM/yyyy"), "")}
            }
        End Function
    End Class
End Namespace
