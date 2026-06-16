using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class MovimientoDataService : GenericDataService<Movimiento>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public MovimientoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Movimiento> Add(Movimiento entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                AttachNavigations(context, entity);
                var created = await context.Set<Movimiento>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Movimiento> Update(int id, Movimiento entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                AttachNavigations(context, entity);
                var updated = context.Set<Movimiento>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Movimiento>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Movimiento>()
                    .Include(m => m.Remedida)
                    .Include(m => m.TipoMovimiento)
                    .ToListAsync();
            }
        }

        private static void AttachNavigations(VemarDbContext context, Movimiento entity)
        {
            if (entity.Remedida != null)
                context.Attach(entity.Remedida);
            if (entity.TipoMovimiento != null)
                context.Attach(entity.TipoMovimiento);
        }
    }
}
