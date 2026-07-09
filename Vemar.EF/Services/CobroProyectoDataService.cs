using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class CobroProyectoDataService : GenericDataService<CobroProyecto>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public CobroProyectoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<CobroProyecto> Add(CobroProyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Proyecto != null)
                    context.Attach(entity.Proyecto);
                var created = await context.Set<CobroProyecto>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<CobroProyecto> Update(int id, CobroProyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Proyecto != null)
                    context.Attach(entity.Proyecto);
                var updated = context.Set<CobroProyecto>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<CobroProyecto>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<CobroProyecto>()
                    .Include(c => c.Proyecto)
                    .ToListAsync();
            }
        }
    }
}
