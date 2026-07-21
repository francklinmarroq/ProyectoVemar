Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class TramitesProyectoReport : Inherits RdlcReportBase(Of Tramite)

        Private ReadOnly _proyectoNombre As String

        Public Sub New(proyectoNombre As String)
            _proyectoNombre = proyectoNombre
        End Sub

        Protected Overrides ReadOnly Property ReportTitle As String
            Get
                Return $"Trámites — {_proyectoNombre}"
            End Get
        End Property

        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Tipo", "Tipo", "2in"),
                    New ReportColumn("Estado", "Estado", "1.5in"),
                    New ReportColumn("Fecha", "Fecha", "1in"),
                    New ReportColumn("Descripción", "Descripcion", "5in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Tramite) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Tipo", If(item.TipoTramite?.Nombre, "")},
                {"Estado", If(item.EstadoTramite?.Estado, "")},
                {"Fecha", If(item.Fecha.HasValue, item.Fecha.Value.ToString("dd/MM/yyyy"), "")},
                {"Descripcion", If(item.Descripcion, "")}
            }
        End Function
    End Class
End Namespace
