using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class AsignacionDataService : GenericDataService<Asignacion>
    {
        private readonly VemarDbContextFactory _contextFactory;

        public AsignacionDataService(VemarDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<Asignacion> Add(Asignacion entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                AttachNavigations(context, entity);
                var created = await context.Set<Asignacion>().AddAsync(entity);
                await context.SaveChangesAsync();
                return created.Entity;
            }
        }

        public override async Task<Asignacion> Update(int id, Asignacion entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                AttachNavigations(context, entity);
                var updated = context.Set<Asignacion>().Update(entity);
                await context.SaveChangesAsync();
                return updated.Entity;
            }
        }

        public override async Task<IEnumerable<Asignacion>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Set<Asignacion>()
                    .Include(a => a.Colaborador)
                    .Include(a => a.Proyecto)
                        .ThenInclude(p => p.Cliente)
                    .ToListAsync();
            }
        }

        private static void AttachNavigations(VemarDbContext context, Asignacion entity)
        {
            if (entity.Colaborador != null)
                context.Attach(entity.Colaborador);
            if (entity.Proyecto != null)
                context.Attach(entity.Proyecto);
        }
    }
}
