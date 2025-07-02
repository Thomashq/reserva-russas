using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Services.Base
{
    public interface IBaseService<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity?> UpdateAsync(TKey id, TEntity entity);
        Task<bool> DeleteAsync(TKey id);
        Task<bool> ExistsAsync(TKey id);
    }
}
