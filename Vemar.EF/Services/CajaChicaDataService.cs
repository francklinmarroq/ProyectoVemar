using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class CajaChicaDataService : GenericDataService<CajaChica>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public CajaChicaDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<CajaChica> Add(CajaChica entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Remedida != null) context.Entry(entity.Remedida).State = EntityState.Unchanged;
                if (entity.Proyecto != null)  context.Entry(entity.Proyecto).State  = EntityState.Unchanged;
                var created = await context.Set<CajaChica>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<CajaChica> Update(int id, CajaChica entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Remedida != null) context.Entry(entity.Remedida).State = EntityState.Unchanged;
                if (entity.Proyecto  != null) context.Entry(entity.Proyecto).State  = EntityState.Unchanged;
                var updated = context.Set<CajaChica>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<CajaChica>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<CajaChica>()
                    .Include(c => c.Remedida)
                    .Include(c => c.Proyecto)
                    .ToListAsync();
            }
        }
    }
}
