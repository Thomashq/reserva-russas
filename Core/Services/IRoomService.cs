// RR.Core/Services/IRoomService.cs
using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IRoomService
    {
        Task<Rooms> AddAsync(Rooms room);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Rooms>> GetAllAsync();
        Task<Rooms?> GetRoomById(int id);
        Task<Rooms?> GetRoomByName(string name);
        Task<Rooms?> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end);
        Task<Rooms?> UpdateAsync(Rooms room);
    }
}
