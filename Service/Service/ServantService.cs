using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Services.Base;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class ServantService : IServantService
    {
        private readonly ApplicationDbContext _context;

        public ServantService(ApplicationDbContext context) { _context = context; }

        public async Task<bool> AddAsync(Servant servant)
        {
            servant.IsActive = true;
            await _context.Servant.AddAsync(servant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Servant> GetServantById(int id)
        {
            var servant = await _context.Servant.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return servant;
        }

        public async Task<Servant> GetServantByAccountId(int id)
        {
            var entity = await _context.Servant.FirstOrDefaultAsync(x => x.AccountId == id && x.IsActive);
            return entity;
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

        public async Task<bool> DeleteAsync(int id)
        {
            var servant = await _context.Servant.FindAsync(id);
            if (servant == null) return false;

            servant.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
