using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicerBeat.Models.Databases
{
    public interface IRepository<T>
    where T : class, IEntity
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);
    }
}