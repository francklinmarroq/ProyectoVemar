Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class AsignacionesProyectoReport : Inherits RdlcReportBase(Of Asignacion)

        Private ReadOnly _proyectoNombre As String

        Public Sub New(proyectoNombre As String)
            _proyectoNombre = proyectoNombre
        End Sub

        Protected Overrides ReadOnly Property ReportTitle As String
            Get
                Return $"Asignaciones — {_proyectoNombre}"
            End Get
        End Property

        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Colaborador", "Colaborador", "2.5in"),
                    New ReportColumn("Cliente", "Cliente", "2in"),
                    New ReportColumn("F. Inicio", "FechaAsignacion", "1in"),
                    New ReportColumn("F. Fin", "FechaFinalizacion", "1in"),
                    New ReportColumn("Observaciones", "Observaciones", "3in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Asignacion) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Colaborador", If(item.Colaborador?.Nombre, "")},
                {"Cliente", If(item.Cliente?.Nombre, "")},
                {"FechaAsignacion", item.FechaAsignacion.ToString("dd/MM/yyyy")},
                {"FechaFinalizacion", If(item.FechaFinalizacion?.ToString("dd/MM/yyyy"), "")},
                {"Observaciones", If(item.Observaciones, "")}
            }
        End Function
    End Class
End Namespace
