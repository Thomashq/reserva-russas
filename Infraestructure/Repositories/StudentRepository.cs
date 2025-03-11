using Core.Repositories;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RR.Infraestructure.Repositories
{
    public class StudentRepository:IRepository<Student>
    {
        private readonly DataContext _context;

        public StudentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Set<Student>().ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Student>().FindAsync(id);
        }

        public async Task AddAsync(Student entity)
        {
            entity.CreationDate = DateTime.UtcNow;
            await _context.Set<Student>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student entity)
        {
            _context.Set<Student>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Student>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Student>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Student, bool>>? filter = null)
        {
            var query = _context.Set<Student>().AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
