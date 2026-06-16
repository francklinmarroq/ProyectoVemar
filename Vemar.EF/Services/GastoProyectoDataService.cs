using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class GastoProyectoDataService : GenericDataService<GastoProyecto>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public GastoProyectoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<GastoProyecto> Add(GastoProyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Proyecto != null) context.Entry(entity.Proyecto).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                var created = await context.Set<GastoProyecto>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<GastoProyecto> Update(int id, GastoProyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Proyecto != null) context.Entry(entity.Proyecto).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                var updated = context.Set<GastoProyecto>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<GastoProyecto>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<GastoProyecto>()
                    .Include(g => g.Proyecto)
                    .ToListAsync();
            }
        }
    }
}
