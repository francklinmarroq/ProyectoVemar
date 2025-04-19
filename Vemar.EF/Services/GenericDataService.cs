using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vemar.Domain;

namespace Vemar.EF.Services
{
    public class GenericDataService<T> : IDataService<T> where T : DomainObject
    {
        private readonly VemarDbContextFactory _contextFactory;

        public GenericDataService(VemarDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<T> Add(T entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                var createdEntity = await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();
                return createdEntity.Entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                T entity = await context.Set<T>()
                    .FirstOrDefaultAsync((e) => e.Id == id);
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<T> GetById(int id)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                T entity = await context.Set<T>()
                    .FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>()
                    .ToListAsync();
                return entities;
            }
        }


        public async Task<T> Update(int id, T entity)
        {
            using (VemarDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                var updatedEntity = context.Set<T>().Update(entity);
                await context.SaveChangesAsync();
                return updatedEntity.Entity;
            }
        }
    }
}
