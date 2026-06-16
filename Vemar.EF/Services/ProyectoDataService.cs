using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class ProyectoDataService : GenericDataService<Proyecto>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public ProyectoDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Proyecto> Add(Proyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                AttachNavigations(context, entity);
                var created = await context.Set<Proyecto>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Proyecto> Update(int id, Proyecto entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                AttachNavigations(context, entity);
                var updated = context.Set<Proyecto>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Proyecto>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Proyecto>()
                    .Include(p => p.Cliente)
                    .Include(p => p.Zonificacion)
                    .Include(p => p.CategoriaProyecto)
                    .ToListAsync();
            }
        }

        private static void AttachNavigations(VemarDbContext context, Proyecto entity)
        {
            if (entity.Cliente != null)
                context.Attach(entity.Cliente);
            if (entity.Zonificacion != null)
                context.Attach(entity.Zonificacion);
            if (entity.CategoriaProyecto != null)
                context.Attach(entity.CategoriaProyecto);
        }
    }
}
