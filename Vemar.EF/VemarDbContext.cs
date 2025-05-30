using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vemar.Domain;

namespace Vemar.EF
{
    public class VemarDbContext : DbContext
    {
        DbSet<Asignacion> Asignaciones { get; set; }
        DbSet<Avance> Avances { get; set; }
        DbSet<CategoriaProyecto> CategoriasProyectos { get; set; }
        DbSet<Cliente> Clientes { get; set; }
        DbSet<CobroRemedida> CobrosRemedidas { get; set; }
        DbSet<Colaborador> Colaboradores { get; set; }
        DbSet<Contratista> Contratistas { get; set; }
        DbSet<Contrato> Contratos { get; set; }
        DbSet<EstadoTramite> EstadosTramites { get; set; }
        DbSet<GastoRemedida> GastosRemedidas { get; set; }
        DbSet<Movimiento> Movimientos { get; set; }
        DbSet<PagoContrato> PagosContratos { get; set; }
        DbSet<Proyecto> Proyectos { get; set; }
        DbSet<Remedida> Remedidas { get; set; }
        DbSet<TipoMovimiento> TiposMovimientos { get; set; }
        DbSet<TipoTramite> TiposTramites { get; set; }
        DbSet<TipoUsuario> TiposUsuarios { get; set; }
        DbSet<Tramite> Tramites { get; set; }
        DbSet<Usuario> Usuarios { get; set; }
        DbSet<Zonificacion> Zonificaciones { get; set; }
        public VemarDbContext(DbContextOptions options) : base(options) { }
    }
}
