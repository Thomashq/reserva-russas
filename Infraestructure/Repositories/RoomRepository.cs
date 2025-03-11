using Core.Repositories;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RR.Infraestructure.Repositories
{
    public class RoomsRepository : IRepository<Rooms>
    { 
        private readonly DataContext _context;

        public RoomsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rooms>> GetAllAsync()
        {
            return await _context.Set<Rooms>().ToListAsync();
        }

        public async Task<Rooms?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Rooms>().FindAsync(id);
        }

        public async Task AddAsync(Rooms entity)
        {
            await _context.Set<Rooms>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rooms entity)
        {
            _context.Set<Rooms>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Rooms>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Rooms>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Rooms, bool>>? filter = null)
        {
            var query = _context.Set<Rooms>().AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
