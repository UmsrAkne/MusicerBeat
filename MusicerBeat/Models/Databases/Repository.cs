using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MusicerBeat.Models.Databases
{
    public class Repository<T> : IRepository<T>
    where T : class, IEntity
    {
        private readonly DatabaseContext context;
        private readonly DbSet<T> dbSet;

        public Repository(DatabaseContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}