using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IEquipmentService
    {
        Task<IEnumerable<Equipment>> GetAllAsync();
        Task<Equipment?> GetByIdAsync(int id);
        Task<Equipment> AddAsync(Equipment equipment);
        Task<Equipment?> UpdateAsync(Equipment equipment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Equipment>> GetEquipmentsByRoomId(int roomId);
    }
}
