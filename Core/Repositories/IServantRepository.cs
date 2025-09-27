using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IServantRepository
    {
        Task<IEnumerable<Servant>> GetAllAsync();
        Task<bool> AddAsync(Servant servant);
        Task<Servant> GetServantById(int id);
        Task<Servant> UpdateAsync(Servant servant);
        Task<bool> DeleteAsync(int id);
        Task<Servant> GetServantByAccountId(int id);
    }
}
