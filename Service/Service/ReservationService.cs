using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservationService>? _logger;

        public ReservationService(ApplicationDbContext context, ILogger<ReservationService>? logger = null) { _context = context; _logger = logger; }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
            => await _context.Reservation.Where(x => x.IsActive).ToListAsync();

        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            if (reservation is null) throw new ArgumentNullException(nameof(reservation));
            if (reservation.RoomId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.RoomId));
            if (reservation.AccountId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.AccountId));
            if (reservation.StartTime >= reservation.EndTime) throw new ArgumentException("StartTime must be before EndTime.");

            var acc = await _context.Account.FirstOrDefaultAsync(a => a.Id == reservation.AccountId);
            if (acc is not null && acc.AccountPermission == (int)EAccountPermission.Student)
                throw new UnauthorizedAccessException("Alunos não podem reservar salas.");

            var available = await IsRoomAvailable(reservation.RoomId, reservation.StartTime, reservation.EndTime);
            if (!available)
                throw new InvalidOperationException("Room is not available for the selected period.");

            reservation.Status = 0; // Criado
            reservation.IsActive = true;

            var entry = await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var created = entry.Entity;
            _logger?.LogInformation("Reservation created: {ReservationId} Room {RoomId} ({Start} - {End})",
                created.Id, created.RoomId, created.StartTime, created.EndTime);

            return created;
        }

        public async Task<Reservation> ApproveReservation(int id)
        {
            var reservation = await GetReservationById(id);
            if (reservation is null) return null!;

            reservation.Status = 1; // Aprovado
            var updated = await UpdateAsync(reservation);

            if (updated is null)
            {
                _logger?.LogError("Failed to approve reservation {ReservationId}", id);
                return null!;
            }

            _logger?.LogInformation("Reservation approved: {ReservationId}", id);
            return updated;
        }

        public async Task<Reservation> GetReservationById(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return await _context.Reservation.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            var existingReservation = await _context.Reservation.FindAsync(reservation.Id);
            if (existingReservation == null) return null;

            existingReservation.RoomId = reservation.RoomId;
            existingReservation.AccountId = reservation.AccountId;
            existingReservation.StartTime = reservation.StartTime;
            existingReservation.EndTime = reservation.EndTime;
            existingReservation.Status = reservation.Status;
            existingReservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingReservation;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByRoomId(int roomId)
        {
            if (roomId <= 0) throw new ArgumentOutOfRangeException(nameof(roomId));
            return await _context.Reservation.Where(x => x.RoomId == roomId && x.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByAccountId(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            return await _context.Reservation.Where(x => x.AccountId == accountId && x.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByPeriod(DateTime start, DateTime end)
        {
            return await _context.Reservation
                .Where(x => x.IsActive && x.StartTime >= start && x.EndTime <= end)
                .ToListAsync();
        }

        public async Task<bool> CancelReservation(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var reservation = await _context.Reservation.FirstOrDefaultAsync(x => x.Id == id);
            if (reservation is null || !reservation.IsActive)
            {
                _logger?.LogWarning("Attempt to cancel non-existent or inactive reservation {ReservationId}", id);
                return false;
            }

            reservation.Status = 2; // Rejeitado/cancelado
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Reservation cancelled: {ReservationId}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null) return false;

            reservation.IsActive = false;
            await _context.SaveChangesAsync();

            _logger?.LogInformation("Reservation deleted (soft): {ReservationId}", id);
            return true;
        }

        public async Task<bool> IsRoomAvailable(int roomId, DateTime start, DateTime end)
        {
            return !await _context.Reservation.AnyAsync(r => r.RoomId == roomId && r.IsActive && r.StartTime < end && r.EndTime > start);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsBySeriesId(int seriesId)
        {
            if (seriesId <= 0) throw new ArgumentOutOfRangeException(nameof(seriesId));
            return await _context.Reservation.Where(x => x.SeriesId == seriesId && x.IsActive).ToListAsync();
        }
    }
}
