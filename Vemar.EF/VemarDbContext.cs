using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vemar.Domain;

namespace Vemar.EF
{
    class VemarDbContext : DbContext
    {
        DbSet<Colaborador> Colaboradores { get; set; }
        DbSet<Movimiento> Movimientos { get; set; }
        DbSet<Remedida> Remedidas { get; set; } 
        DbSet<TipoMovimiento> TiposMovimientos { get; set; }
        DbSet<TipoUsuario> TiposUsuarios { get; set; }
        DbSet<Usuario> Usuarios { get; set; }
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=FMarroquin\\SQLEXPRESS;Database=Vemar;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
