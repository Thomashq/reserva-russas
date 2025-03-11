using Core.Repositories;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RR.Infraestructure.Repositories
{
    public class ManagerRepository:IRepository<Manager>
    {
        private readonly DataContext _context;

        public ManagerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Manager>> GetAllAsync()
        {
            return await _context.Set<Manager>().ToListAsync();
        }

        public async Task<Manager?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Manager>().FindAsync(id);
        }

        public async Task AddAsync(Manager entity)
        {
            await _context.Set<Manager>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Manager entity)
        {
            _context.Set<Manager>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Manager>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Manager>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Manager, bool>>? filter = null)
        {
            var query = _context.Set<Manager>().AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
