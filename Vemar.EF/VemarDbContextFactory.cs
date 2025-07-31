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
                .UseSqlServer("Server=DESKTOPFRANCKLI\\SQLEXPRESS;Database=Vemar;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            return new VemarDbContext(options);
        }
    }
}
