using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class ReservationSeriesRepository : IReservationSeriesRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationSeriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddSeries(ReservationSeries series)
        {
            var entity = await _context.ReservationSeries.AddAsync(series);
            await _context.SaveChangesAsync();
            return entity.Entity.Id;
        }

        public async Task<bool> DeleteSeries(ReservationSeries series)
        {
            var entity = await _context.ReservationSeries.FindAsync(series.Id);

            if (entity == null)
            {
                return false;            
            }
            
            entity.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ReservationSeries?> GetSeriesById(int seriesId)
        {
            var entity = await _context.ReservationSeries.FindAsync(seriesId);

            return entity;
        }

        public async Task<ReservationSeries> UpdateSeries(ReservationSeries series)
        {
            var entity = await _context.ReservationSeries.FindAsync(series.Id);

            if (entity == null) throw new Exception("Série não encontrada");

            entity.Title = series.Title;
            entity.Description = series.Description;
            entity.WindowStart = series.WindowStart;
            entity.WindowEnd = series.WindowEnd;
            entity.RecurrenceRule = series.RecurrenceRule;
            entity.DaysOfWeek = series.DaysOfWeek;
            entity.TimeStart = series.TimeStart;
            entity.TimeEnd = series.TimeEnd;
            entity.SeriesStatus = series.SeriesStatus;
            entity.RoomId = series.RoomId;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
