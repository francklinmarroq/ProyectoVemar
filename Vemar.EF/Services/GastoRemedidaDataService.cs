using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class GastoRemedidaDataService : GenericDataService<GastoRemedida>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public GastoRemedidaDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<GastoRemedida> Add(GastoRemedida entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Remedida != null) context.Entry(entity.Remedida).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                var created = await context.Set<GastoRemedida>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<GastoRemedida> Update(int id, GastoRemedida entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Remedida != null) context.Entry(entity.Remedida).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                var updated = context.Set<GastoRemedida>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<GastoRemedida>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<GastoRemedida>()
                    .Include(g => g.Remedida)
                    .ToListAsync();
            }
        }
    }
}
