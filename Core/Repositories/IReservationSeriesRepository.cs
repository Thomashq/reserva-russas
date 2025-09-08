using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IReservationSeriesRepository
    {
        Task<int> AddSeries(ReservationSeries series);
        Task<ReservationSeries?> GetSeriesById(int seriesId);
        Task<ReservationSeries> UpdateSeries(ReservationSeries series);
        Task<bool> DeleteSeries(ReservationSeries series);
    }
}
