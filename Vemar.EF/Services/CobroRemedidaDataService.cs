using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class CobroRemedidaDataService : GenericDataService<CobroRemedida>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public CobroRemedidaDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<CobroRemedida> Add(CobroRemedida entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Remedida != null)
                    context.Attach(entity.Remedida);
                var created = await context.Set<CobroRemedida>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<CobroRemedida> Update(int id, CobroRemedida entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Remedida != null)
                    context.Attach(entity.Remedida);
                var updated = context.Set<CobroRemedida>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<CobroRemedida>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<CobroRemedida>()
                    .Include(c => c.Remedida)
                    .ToListAsync();
            }
        }
    }
}
