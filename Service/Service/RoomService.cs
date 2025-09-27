using Microsoft.Extensions.Logging;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _rooms;
        private readonly ILogger<RoomService>? _logger;

        public RoomService(IRoomRepository rooms, ILogger<RoomService>? logger = null)
        {
            _rooms = rooms;
            _logger = logger;
        }

        public async Task<Rooms> AddAsync(Rooms room)
        {
            if (room is null) throw new ArgumentNullException(nameof(room));
            if (string.IsNullOrWhiteSpace(room.Name))
                throw new ArgumentException("Room name is required.", nameof(room));
            if (room.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.", nameof(room));

            // (Opcional) checar duplicidade de nome ativo
            var existing = await _rooms.GetRoomByName(room.Name);
            if (existing is not null)
                throw new InvalidOperationException($"Room '{room.Name}' already exists.");

            var created = await _rooms.AddAsync(room);
            _logger?.LogInformation("Room created: {RoomId} - {Name}", created.Id, created.Name);
            return created;
        }

        public Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _rooms.DeleteAsync(id); 
        }

        public Task<IEnumerable<Rooms>> GetAllAsync()
            => _rooms.GetAllAsync();

        public Task<Rooms?> GetRoomById(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return _rooms.GetRoomById(id);
        }

        public Task<Rooms?> GetRoomByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            return _rooms.GetRoomByName(name);
        }

        public Task<Rooms?> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (start >= end) throw new ArgumentException("Start must be before End.");
            return _rooms.GetRoomsReservationsByPeriod(id, start, end);
        }

        public async Task<Rooms?> UpdateAsync(Rooms room)
        {
            if (room is null) throw new ArgumentNullException(nameof(room));
            if (room.Id <= 0) throw new ArgumentOutOfRangeException(nameof(room.Id));
            if (string.IsNullOrWhiteSpace(room.Name))
                throw new ArgumentException("Room name is required.", nameof(room));
            if (room.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.", nameof(room));

            var updated = await _rooms.UpdateAsync(room);
            if (updated is null)
            {
                _logger?.LogWarning("Attempt to update non-existent room {RoomId}", room.Id);
                return null;
            }

            _logger?.LogInformation("Room updated: {RoomId} - {Name}", updated.Id, updated.Name);
            return updated;
        }
    }
}
