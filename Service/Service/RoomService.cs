using Core.Repositories;
using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class RoomService:IRoomService
    {
        private readonly IRepository<Rooms> _repository;

        public RoomService(IRepository<Rooms> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RoomDTO>> GetAllAsync()
        {
            var rooms = await _repository.GetAllAsync();
            return rooms.Select(r => new RoomDTO { Id = r.Id, Name = r.Name, Capacity = r.Capacity, ManagerId = r.ManagerId });
        }

        public async Task<RoomDTO?> GetByIdAsync(Guid id)
        {
            var room = await _repository.GetByIdAsync(id);
            return room != null ? new RoomDTO { Id = room.Id, Name = room.Name, Capacity = room.Capacity, ManagerId = room.ManagerId } : null;
        }

        public async Task<RoomDTO> CreateAsync(RoomDTO dto)
        {
            var room = new Rooms { Id = dto.Id, Name = dto.Name, Capacity = dto.Capacity, ManagerId = dto.ManagerId };
            await _repository.AddAsync(room);
            return dto;
        }

        public async Task<RoomDTO?> UpdateAsync(Guid id, RoomDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Capacity = dto.Capacity;
            existing.ManagerId = dto.ManagerId;
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
