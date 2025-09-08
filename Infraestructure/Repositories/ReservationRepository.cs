using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;
        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            reservation.IsActive = true;
            var entry = await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null) return false;

            reservation.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservation.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Reservation> GetReservationById(int id)
        {
            return await _context.Reservation.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByAccountId(int accountId)
        {
            return await _context.Reservation.Where(x => x.AccountId == accountId && x.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByPeriod(DateTime start, DateTime end)
        {
            return await _context.Reservation
                .Where(r => r.StartTime < end && r.EndTime > start && r.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByRoomId(int roomId)
        {
            return await _context.Reservation.Where(x => x.IsActive && x.RoomId == roomId).ToListAsync();
        }

        public async Task<bool> IsRoomAvailable(int roomId, DateTime start, DateTime end)
        {
            return await Task.FromResult(!_context.Reservation.Any(r => r.RoomId == roomId && r.IsActive && r.StartTime < end && r.EndTime > start));
        }

        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            var existingReservation = await _context.Reservation.FindAsync(reservation.Id);
            if (existingReservation == null) return null;
            existingReservation.RoomId = reservation.RoomId;
            existingReservation.AccountId = reservation.AccountId;
            existingReservation.StartTime = reservation.StartTime;
            existingReservation.EndTime = reservation.EndTime;
            existingReservation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existingReservation;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsBySeriesId(int seriesId)
        {
            return await _context.Reservation
                .Where(r => r.SeriesId == seriesId && r.IsActive)
                .ToListAsync();
        }
    }
}
