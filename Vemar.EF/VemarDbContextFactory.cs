using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vemar.EF
{
    public class VemarDbContextFactory : IDesignTimeDbContextFactory<VemarDbContext>
    {
        public VemarDbContextFactory()
        {
        }

        public VemarDbContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<VemarDbContext>()
                .UseSqlServer(ConnectionConfig.ConnectionString)
                .Options;

            return new VemarDbContext(options);
        }

        /// <summary>
        /// Aplica todas las migraciones pendientes. Si la BD no existe la crea.
        /// Llamar al inicio de la aplicación para garantizar que el esquema esté actualizado.
        /// </summary>
        public static async Task ApplyMigrationsAsync()
        {
            var factory = new VemarDbContextFactory();
            using var ctx = factory.CreateDbContext();
            await ctx.Database.MigrateAsync();
        }
    }
}
