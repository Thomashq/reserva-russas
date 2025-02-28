using Core.Repositories;
using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class ManagerService : IManagerService
    {
        private readonly IRepository<Manager> _repository;

        public ManagerService(IRepository<Manager> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ManagerDTO>> GetAllAsync()
        {
            var managers = await _repository.GetAllAsync();
            return managers.Select(m => new ManagerDTO { Id = m.Id, AccountId = m.AccountId, ManagedRooms = m.ManagedRooms });
        }

        public async Task<ManagerDTO?> GetByIdAsync(Guid id)
        {
            var manager = await _repository.GetByIdAsync(id);
            return manager != null ? new ManagerDTO { Id = manager.Id, AccountId = manager.AccountId, ManagedRooms = manager.ManagedRooms } : null;
        }

        public async Task<ManagerDTO> CreateAsync(ManagerDTO dto)
        {
            var manager = new Manager { Id = dto.Id, AccountId = dto.AccountId, ManagedRooms = dto.ManagedRooms };
            await _repository.AddAsync(manager);
            return dto;
        }

        public async Task<ManagerDTO?> UpdateAsync(Guid id, ManagerDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.ManagedRooms = dto.ManagedRooms;
            await _repository.UpdateAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
