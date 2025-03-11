using System.Linq.Expressions;

namespace Core.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);

        Task<(IEnumerable<T>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null);
    }
}
