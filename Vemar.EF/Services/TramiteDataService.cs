using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class TramiteDataService : GenericDataService<Tramite>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public TramiteDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Tramite> Add(Tramite entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                AttachNavigations(context, entity);
                var created = await context.Set<Tramite>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Tramite> Update(int id, Tramite entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                AttachNavigations(context, entity);
                var updated = context.Set<Tramite>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Tramite>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Tramite>()
                    .Include(t => t.TipoTramite)
                    .Include(t => t.EstadoTramite)
                    .Include(t => t.Proyecto)
                    .ToListAsync();
            }
        }

        private static void AttachNavigations(VemarDbContext context, Tramite entity)
        {
            if (entity.TipoTramite != null)
                context.Entry(entity.TipoTramite).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            if (entity.EstadoTramite != null)
                context.Entry(entity.EstadoTramite).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            if (entity.Proyecto != null)
                context.Entry(entity.Proyecto).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }
    }
}
