using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class PagoContratoDataService : GenericDataService<PagoContrato>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public PagoContratoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<PagoContrato> Add(PagoContrato entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Contrato != null)
                    context.Attach(entity.Contrato);
                var created = await context.Set<PagoContrato>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<PagoContrato> Update(int id, PagoContrato entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Contrato != null)
                    context.Attach(entity.Contrato);
                var updated = context.Set<PagoContrato>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<PagoContrato>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<PagoContrato>()
                    .Include(p => p.Contrato)
                        .ThenInclude(c => c.Proyecto)
                    .Include(p => p.Contrato)
                        .ThenInclude(c => c.Contratista)
                    .ToListAsync();
            }
        }
    }
}
