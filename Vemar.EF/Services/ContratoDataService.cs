using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class ContratoDataService : GenericDataService<Contrato>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public ContratoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Contrato> Add(Contrato entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                AttachNavigations(context, entity);
                var created = await context.Set<Contrato>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Contrato> Update(int id, Contrato entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                AttachNavigations(context, entity);
                var updated = context.Set<Contrato>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Contrato>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Contrato>()
                    .Include(c => c.Contratista)
                    .Include(c => c.Proyecto)
                    .ToListAsync();
            }
        }

        private static void AttachNavigations(VemarDbContext context, Contrato entity)
        {
            if (entity.Contratista != null)
                context.Attach(entity.Contratista);
            if (entity.Proyecto != null)
                context.Attach(entity.Proyecto);
        }
    }
}
