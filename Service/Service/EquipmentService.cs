using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RR.Core.Entities;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EquipmentService>? _logger;

        public EquipmentService(
            ApplicationDbContext context,
            ILogger<EquipmentService>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            return await _context.Equipment
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            return await _context.Equipment
                .Include(e => e.RoomEquipments)
                    .ThenInclude(re => re.RoomDetails)
                        .ThenInclude(rd => rd.Room)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<Equipment> AddAsync(Equipment equipment)
        {
            if (equipment is null)
                throw new ArgumentNullException(nameof(equipment));

            if (string.IsNullOrWhiteSpace(equipment.Name))
                throw new ArgumentException("Equipment name is required.", nameof(equipment.Name));

            // Verifica se já existe um equipamento com o mesmo nome
            var exists = await _context.Equipment
                .AnyAsync(e => e.Name.ToLower() == equipment.Name.ToLower() && e.IsActive);

            if (exists)
                throw new InvalidOperationException($"Equipment with name '{equipment.Name}' already exists.");

            equipment.IsActive = true;
            equipment.CreatedAt = DateTime.UtcNow;

            var entry = await _context.Equipment.AddAsync(equipment);
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment created: {EquipmentId} - {EquipmentName}",
                entry.Entity.Id, entry.Entity.Name);

            return entry.Entity;
        }

        public async Task<Equipment?> UpdateAsync(Equipment equipment)
        {
            if (equipment is null)
                throw new ArgumentNullException(nameof(equipment));

            var existing = await _context.Equipment.FindAsync(equipment.Id);
            if (existing is null || !existing.IsActive)
                return null;

            // Verifica se o novo nome já existe em outro equipamento
            var nameExists = await _context.Equipment
                .AnyAsync(e => e.Id != equipment.Id &&
                              e.Name.ToLower() == equipment.Name.ToLower() &&
                              e.IsActive);

            if (nameExists)
                throw new InvalidOperationException($"Equipment with name '{equipment.Name}' already exists.");

            existing.Name = equipment.Name;
            existing.Description = equipment.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment updated: {EquipmentId} - {EquipmentName}",
                existing.Id, existing.Name);

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment is null || !equipment.IsActive)
                return false;

            // Verifica se há reservas ativas para este equipamento
            var hasActiveReservations = await _context.EquipmentReservation
                .AnyAsync(er => er.EquipmentId == id &&
                               er.IsActive &&
                               er.Status != 2 && // Não canceladas
                               er.EndTime > DateTime.UtcNow);

            if (hasActiveReservations)
                throw new InvalidOperationException("Cannot delete equipment with active reservations.");

            equipment.IsActive = false;
            equipment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment deleted (soft): {EquipmentId} - {EquipmentName}",
                equipment.Id, equipment.Name);

            return true;
        }

        public async Task<IEnumerable<Equipment>> GetEquipmentsByRoomId(int roomId)
        {
            if (roomId <= 0) throw new ArgumentOutOfRangeException(nameof(roomId));

            // Busca RoomDetails pela sala
            var roomDetails = await _context.RoomDetails
                .Include(rd => rd.RoomEquipments)
                    .ThenInclude(re => re.Equipment)
                .FirstOrDefaultAsync(rd => rd.RoomId == roomId && rd.IsActive);

            if (roomDetails is null)
                return Enumerable.Empty<Equipment>();

            return roomDetails.RoomEquipments
                .Where(re => re.IsActive && re.Equipment.IsActive)
                .Select(re => re.Equipment)
                .OrderBy(e => e.Name)
                .ToList();
        }
    }
}
