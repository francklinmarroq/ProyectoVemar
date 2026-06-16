using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class AvanceDataService : GenericDataService<Avance>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public AvanceDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Avance> Add(Avance entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                if (entity.Proyecto != null)
                    context.Attach(entity.Proyecto);
                var created = await context.Set<Avance>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Avance> Update(int id, Avance entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                if (entity.Proyecto != null)
                    context.Attach(entity.Proyecto);
                var updated = context.Set<Avance>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Avance>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Avance>()
                    .Include(a => a.Proyecto)
                    .ToListAsync();
            }
        }
    }
}
