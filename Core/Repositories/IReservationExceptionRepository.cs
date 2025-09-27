using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IReservationExceptionRepository
    {
        Task<int> AddException(ReservationException exception);
        Task<ReservationException?> GetExceptionById(int exceptionId);
        Task<IEnumerable<ReservationException>> GetExceptionsBySeriesId(int seriesId);
        Task<ReservationException> UpdateException(ReservationException exception);
        Task<bool> DeleteException(ReservationException exception);
    }
}
