using RR.Core.DTOs;

namespace RR.Core.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDTO>> GetAllAsync();
        Task<RoomDTO?> GetByIdAsync(Guid id);
        Task<RoomDTO> CreateAsync(RoomDTO dto);
        Task<RoomDTO?> UpdateAsync(Guid id, RoomDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
