using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<Rooms>> GetAllAsync()
        {
            return await _context.Rooms.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Rooms?> GetRoomById(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return room;
        }

        public async Task<Rooms?> GetRoomByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
            return room;
        }

        public async Task<Rooms?> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end)
        {
            var room = await _context.Rooms
                .Include(r => r.Reservations.Where(res => res.StartTime >= start && res.EndTime <= end && res.IsActive))
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            return room;
        }

        public async Task<Rooms> AddAsync(Rooms room)
        {
            room.IsActive = true;
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Rooms?> UpdateAsync(Rooms room)
        {
            var existingRoom = await _context.Rooms.FindAsync(room.Id);
            if (existingRoom == null) return null;

            existingRoom.Name = room.Name;
            existingRoom.Capacity = room.Capacity;
            existingRoom.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
