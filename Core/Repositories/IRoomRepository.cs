using RR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Rooms>> GetAllAsync();
        Task<Rooms> AddAsync(Rooms room);
        Task<Rooms> GetRoomById(int id);
        Task<Rooms> UpdateAsync(Rooms room);
        Task<bool> DeleteAsync(int id);
        Task<Rooms> GetRoomByName(string name);
        Task<Rooms> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end);
    }
}
