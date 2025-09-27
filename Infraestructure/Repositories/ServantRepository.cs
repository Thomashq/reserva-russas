using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class ServantRepository : IServantRepository
    {
        private readonly ApplicationDbContext _context;

        public ServantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Servant servant)
        {
            servant.IsActive = true;
            await _context.Servant.AddAsync(servant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var servant = await _context.Servant.FindAsync(id);
            if (servant == null) return false;

            servant.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Servant>> GetAllAsync()
        {
            return await _context.Servant.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Servant> GetServantByAccountId(int id)
        {
            var entity = await _context.Servant.FirstOrDefaultAsync(x => x.AccountId == id && x.IsActive);
            return entity;
        }

        public async Task<Servant> GetServantById(int id)
        {
            var servant = await _context.Servant.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return servant;
        }

        public async Task<Servant> UpdateAsync(Servant servant)
        {
            var existingServant = await _context.Servant.FindAsync(servant.Id);
            if (existingServant == null) return null;

            existingServant.AccountId = servant.AccountId;
            existingServant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingServant;
        }
    }
}