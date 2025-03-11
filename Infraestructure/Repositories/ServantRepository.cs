using Core.Repositories;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RR.Infraestructure.Repositories
{
    public class ServantRepository:IRepository<Servant>
    {
        private readonly DataContext _context;

        public ServantRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Servant>> GetAllAsync()
        {
            return await _context.Set<Servant>().ToListAsync();
        }

        public async Task<Servant?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Servant>().FindAsync(id);
        }

        public async Task AddAsync(Servant entity)
        {
            await _context.Set<Servant>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Servant entity)
        {
            _context.Set<Servant>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Servant>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Servant>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Servant, bool>>? filter = null)
        {
            var query = _context.Set<Servant>().AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
