using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class EquipmentReservationService : IEquipmentReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EquipmentReservationService>? _logger;

        public EquipmentReservationService(
            ApplicationDbContext context, 
            ILogger<EquipmentReservationService>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<EquipmentReservation>> GetAllAsync()
            => await _context.EquipmentReservation
                .Include(e => e.Equipment)
                .Include(e => e.Account)
                .Where(x => x.IsActive)
                .ToListAsync();

        public async Task<EquipmentReservation?> GetReservationById(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            
            return await _context.EquipmentReservation
                .Include(e => e.Equipment)
                .Include(e => e.Account)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<EquipmentReservation> AddAsync(EquipmentReservation reservation)
        {
            if (reservation is null) 
                throw new ArgumentNullException(nameof(reservation));
            if (reservation.EquipmentId <= 0) 
                throw new ArgumentOutOfRangeException(nameof(reservation.EquipmentId));
            if (reservation.AccountId <= 0) 
                throw new ArgumentOutOfRangeException(nameof(reservation.AccountId));
            if (reservation.StartTime >= reservation.EndTime) 
                throw new ArgumentException("StartTime must be before EndTime.");

            // Verifica se a conta existe e se não é estudante
            var acc = await _context.Account.FirstOrDefaultAsync(a => a.Id == reservation.AccountId);
            if (acc is not null && acc.AccountPermission == (int)EAccountPermission.Student)
                throw new UnauthorizedAccessException("Alunos não podem reservar equipamentos.");

            // Verifica se o equipamento existe
            var equipment = await _context.Equipment.FindAsync(reservation.EquipmentId);
            if (equipment is null)
                throw new InvalidOperationException("Equipamento não encontrado.");

            // Verifica disponibilidade
            var available = await IsEquipmentAvailable(
                reservation.EquipmentId, 
                reservation.StartTime, 
                reservation.EndTime);
            
            if (!available)
                throw new InvalidOperationException("Equipamento não está disponível para o período selecionado.");

            reservation.Status = 0; // Criado
            reservation.IsActive = true;
            reservation.CreatedAt = DateTime.UtcNow;

            var entry = await _context.EquipmentReservation.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var created = entry.Entity;
            _logger?.LogInformation(
                "Equipment reservation created: {ReservationId} Equipment {EquipmentId} ({Start} - {End})",
                created.Id, created.EquipmentId, created.StartTime, created.EndTime);

            // Recarrega com as navegações
            return (await GetReservationById(created.Id))!;
        }

        public async Task<EquipmentReservation?> UpdateAsync(EquipmentReservation reservation)
        {
            var existing = await _context.EquipmentReservation.FindAsync(reservation.Id);
            if (existing is null) return null;

            // Se está mudando o período ou equipamento, verifica disponibilidade
            if (existing.EquipmentId != reservation.EquipmentId ||
                existing.StartTime != reservation.StartTime ||
                existing.EndTime != reservation.EndTime)
            {
                var available = await IsEquipmentAvailable(
                    reservation.EquipmentId,
                    reservation.StartTime,
                    reservation.EndTime,
                    reservation.Id);

                if (!available)
                    throw new InvalidOperationException("Equipamento não está disponível para o novo período.");
            }

            existing.EquipmentId = reservation.EquipmentId;
            existing.AccountId = reservation.AccountId;
            existing.StartTime = reservation.StartTime;
            existing.EndTime = reservation.EndTime;
            existing.Status = reservation.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger?.LogInformation("Equipment reservation updated: {ReservationId}", reservation.Id);
            
            return await GetReservationById(existing.Id);
        }

        public async Task<EquipmentReservation?> ApproveReservation(int id)
        {
            var reservation = await _context.EquipmentReservation.FindAsync(id);
            if (reservation is null || !reservation.IsActive) return null;

            reservation.Status = 1; // Aprovado
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment reservation approved: {ReservationId}", id);
            
            return await GetReservationById(id);
        }

        public async Task<bool> CancelReservation(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var reservation = await _context.EquipmentReservation.FindAsync(id);
            if (reservation is null || !reservation.IsActive)
            {
                _logger?.LogWarning(
                    "Attempt to cancel non-existent or inactive equipment reservation {ReservationId}", id);
                return false;
            }

            reservation.Status = 2; // Cancelado
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment reservation cancelled: {ReservationId}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var reservation = await _context.EquipmentReservation.FindAsync(id);
            if (reservation is null) return false;

            reservation.IsActive = false;
            reservation.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Equipment reservation deleted (soft): {ReservationId}", id);
            return true;
        }

        public async Task<IEnumerable<EquipmentReservation>> GetReservationsByEquipmentId(int equipmentId)
        {
            if (equipmentId <= 0) throw new ArgumentOutOfRangeException(nameof(equipmentId));
            
            return await _context.EquipmentReservation
                .Include(e => e.Equipment)
                .Include(e => e.Account)
                .Where(x => x.EquipmentId == equipmentId && x.IsActive)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<EquipmentReservation>> GetReservationsByAccountId(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            
            return await _context.EquipmentReservation
                .Include(e => e.Equipment)
                .Include(e => e.Account)
                .Where(x => x.AccountId == accountId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EquipmentReservation>> GetReservationsByPeriod(DateTime start, DateTime end)
        {
            return await _context.EquipmentReservation
                .Include(e => e.Equipment)
                .Include(e => e.Account)
                .Where(x => x.IsActive && x.StartTime < end && x.EndTime > start)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsEquipmentAvailable(
            int equipmentId, 
            DateTime start, 
            DateTime end, 
            int? excludeReservationId = null)
        {
            var query = _context.EquipmentReservation
                .Where(r => r.EquipmentId == equipmentId 
                    && r.IsActive 
                    && r.Status != 2 // Não considera reservas canceladas
                    && r.StartTime < end 
                    && r.EndTime > start);

            if (excludeReservationId.HasValue)
            {
                query = query.Where(r => r.Id != excludeReservationId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
