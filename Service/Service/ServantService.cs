using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services.Base;

namespace RR.Service.Service
{
    public class ServantService : IServantService
    {
        private readonly IServantRepository _servantRepository;

        public ServantService(IServantRepository servantRepository)
        {
            _servantRepository = servantRepository;
        }

        public async Task<bool> AddAsync(Servant servant)
        {
            return await _servantRepository.AddAsync(servant);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _servantRepository.DeleteAsync(id);
        }

        public async Task<Servant> GetServantByAccountId(int id)
        {
            return await _servantRepository.GetServantByAccountId(id);
        }

        public async Task<Servant> GetServantById(int id)
        {
            return await _servantRepository.GetServantById(id);
        }

        public async Task<Servant> UpdateAsync(Servant servant)
        {
            return await _servantRepository.UpdateAsync(servant);
        }
    }
}