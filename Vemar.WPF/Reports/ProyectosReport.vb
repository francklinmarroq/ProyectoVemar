Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ProyectosReport : Inherits RdlcReportBase(Of Proyecto)
        Protected Overrides ReadOnly Property ReportTitle As String = "Proyectos"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Nombre", "Nombre", "2in"),
                    New ReportColumn("Cliente", "Cliente", "1.8in"),
                    New ReportColumn("Ubicación", "Ubicacion", "1.5in"),
                    New ReportColumn("Matrícula", "Matricula", "1in"),
                    New ReportColumn("Clave SURE", "ClaveSure", "1in"),
                    New ReportColumn("Área", "Area", "0.8in"),
                    New ReportColumn("Categoría", "Categoria", "1.4in"),
                    New ReportColumn("Zonificación", "Zonificacion", "1.5in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Proyecto) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Nombre", item.Nombre},
                {"Cliente", item.Cliente?.Nombre},
                {"Ubicacion", item.Ubicacion},
                {"Matricula", item.Matricula},
                {"ClaveSure", item.ClaveSure},
                {"Area", item.Area.ToString("N2")},
                {"Categoria", item.CategoriaProyecto?.Nombre},
                {"Zonificacion", item.Zonificacion?.Zonificacion}
            }
        End Function
    End Class
End Namespace
