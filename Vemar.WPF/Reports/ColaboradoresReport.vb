Imports Vemar.Domain

Namespace Vemar.WPF.Reports
    Public Class ColaboradoresReport : Inherits RdlcReportBase(Of Colaborador)
        Protected Overrides ReadOnly Property ReportTitle As String = "Colaboradores"
        Protected Overrides ReadOnly Property IsLandscape As Boolean = True

        Protected Overrides ReadOnly Property Columns As List(Of ReportColumn)
            Get
                Return New List(Of ReportColumn) From {
                    New ReportColumn("ID", "Id", "0.5in"),
                    New ReportColumn("Nombre", "Nombre", "2in"),
                    New ReportColumn("DNI", "Dni", "1.2in"),
                    New ReportColumn("Cargo", "Cargo", "1.5in"),
                    New ReportColumn("Teléfono", "Telefono", "1.1in"),
                    New ReportColumn("Email", "Email", "1.8in"),
                    New ReportColumn("Domicilio", "Domicilio", "2in"),
                    New ReportColumn("F. Nacimiento", "FechaNacimiento", "1.1in")
                }
            End Get
        End Property

        Protected Overrides Function GetRowValues(item As Colaborador) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {"Id", item.Id.ToString()},
                {"Nombre", item.Nombre},
                {"Dni", item.Dni},
                {"Cargo", item.Cargo},
                {"Telefono", item.Telefono},
                {"Email", item.Email},
                {"Domicilio", item.Domicilio},
                {"FechaNacimiento", item.FechaNacimiento.ToString("dd/MM/yyyy")}
            }
        End Function
    End Class
End Namespace
