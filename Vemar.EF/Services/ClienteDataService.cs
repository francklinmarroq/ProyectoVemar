using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class ClienteDataService : GenericDataService<Cliente> 
    {
        private readonly VemarDbContextFactory _contextFactory;
        public ClienteDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<Cliente> GetByRtn(string rtn)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                Cliente entity = await context.Set<Cliente>()
                    .FirstOrDefaultAsync((e) => e.Rtn == rtn);
                return entity;
            }
        }
    }
}
