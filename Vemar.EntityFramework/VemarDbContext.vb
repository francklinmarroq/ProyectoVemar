Imports Microsoft.EntityFrameworkCore
Imports Vemar.Domain

Public Class VemarDbContext : Inherits DbContext

    Public Colaboradores As DbSet(Of Colaborador)
    Public Movimientos As DbSet(Of Movimiento)
    Public Remedidas As DbSet(Of Remedida)
    Public TiposMovimiento As DbSet(Of TipoMovimiento)
    Public TiposUsuario As DbSet(Of TipoUsuario)
    Public Usuarios As DbSet(Of Usuario)

    Protected Overrides Sub OnConfiguring(optionsBuilder As DbContextOptionsBuilder)
        optionsBuilder.UseSqlServer("Server=.\FMARROQUIN\SQLEXPRESS;Database=Vemar;Trusted_Connection=True;")


        MyBase.OnConfiguring(optionsBuilder)
    End Sub

End Class
