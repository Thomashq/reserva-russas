using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class ReservationExceptionRepository : IReservationExceptionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationExceptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddException(ReservationException exception)
        {
            var entity = await _context.ReservationException.AddAsync(exception);
            await _context.SaveChangesAsync();

            return entity.Entity.Id;
        }

        public async Task<bool> DeleteException(ReservationException exception)
        {
            var entity = await _context.ReservationException.FindAsync(exception.Id);

            if (entity == null)
            {
                return false;
            }

            entity.IsActive = false;

            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<ReservationException?> GetExceptionById(int exceptionId)
        {
            return await _context.ReservationException.FindAsync(exceptionId);
        }

        public async Task<IEnumerable<ReservationException>> GetExceptionsBySeriesId(int seriesId)
        {
            return await _context.ReservationException
                .Where(e => e.SeriesId == seriesId && e.IsActive)
                .ToListAsync();
        }

        public async Task<ReservationException> UpdateException(ReservationException exception)
        {
            var entity = await _context.ReservationException.FindAsync(exception.Id);

            if (entity == null)
                throw new KeyNotFoundException("Reservation exception not found.");

            entity.Action = exception.Action;
            entity.OriginalDate = exception.OriginalDate;
            entity.ExceptionDate = exception.ExceptionDate;
            entity.NewStartTime = exception.NewStartTime;
            entity.NewEndTime = exception.NewEndTime;
            entity.NewRoomId = exception.NewRoomId;
            entity.Reason = exception.Reason;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
