using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Rooms> AddAsync(Rooms room)
        {
            room.IsActive = true;

            var entry = await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Rooms>> GetAllAsync()
        {
            return await _context.Rooms.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Rooms> GetRoomById(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            return room;
        }

        public async Task<Rooms> GetRoomByName(string name)
        {
            return await _context.Rooms.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<Rooms> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end)
        {
            return await _context.Rooms
                .Include(r => r.Reservations.Where(res => res.StartTime < end && res.EndTime > start && res.IsActive))
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<Rooms> UpdateAsync(Rooms room)
        {
            var existingRoom = await _context.Rooms.FindAsync(room.Id);
            if (existingRoom == null) return null;
            existingRoom.Name = room.Name;
            existingRoom.Capacity = room.Capacity;
            existingRoom.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingRoom;
        }
    }
}
