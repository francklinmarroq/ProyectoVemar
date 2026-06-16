Imports Microsoft.EntityFrameworkCore
Imports Vemar.Domain

Public Class VemarDbContext : Inherits DbContext

    Public Colaboradores As DbSet(Of Colaborador)
    Public Movimientos As DbSet(Of Movimiento)
    Public Remedidas As DbSet(Of Remedida)
    Public TiposMovimiento As DbSet(Of TipoMovimiento)
    Public TiposUsuario As DbSet(Of TipoUsuario)
    Public Usuarios As DbSet(Of Usuario)
    Public Clientes As DbSet(Of Cliente)
    Public Proyectos As DbSet(Of Proyecto)
    Public Tramites As DbSet(Of Tramite)
    Public Contratistas As DbSet(Of Contratista)
    Public Asignaciones As DbSet(Of Asignacion)
    Public Avances As DbSet(Of Avance)
    Public Contratos As DbSet(Of Contrato)
    Public CobroRemedidas As DbSet(Of CobroRemedida)
    Public GastoRemedidas As DbSet(Of GastoRemedida)
    Public PagoContratos As DbSet(Of PagoContrato)
    Public Zonificaciones As DbSet(Of Zonificacion)
    Public CategoriasProyecto As DbSet(Of CategoriaProyecto)
    Public TiposTramite As DbSet(Of TipoTramite)
    Public EstadosTramite As DbSet(Of EstadoTramite)

    Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
        optionsBuilder.UseSqlServer("Server=.\FMARROQUIN\SQLEXPRESS;Database=Vemar;Trusted_Connection=True;")
        MyBase.OnConfiguring(optionsBuilder)
    End Sub

End Class
