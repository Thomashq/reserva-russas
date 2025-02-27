using Core.Repositories;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RR.Infraestructure.Repositories
{
    public class AccountRepository:IRepository<Account>
    {
        private readonly DataContext _context;

        public AccountRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Set<Account>().ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Account>().FindAsync(id);
        }

        public async Task AddAsync(Account entity)
        {
            await _context.Set<Account>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account entity)
        {
            _context.Set<Account>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Account>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Account>, int)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Account, bool>>? filter = null)
        {
            var query = _context.Set<Account>().AsQueryable();
            if (filter != null)
                query = query.Where(filter);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems);
        }
    }
}
