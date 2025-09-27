using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation> AddAsync(Reservation reservation);
        Task<Reservation> GetReservationById(int id);
        Task<Reservation> UpdateAsync(Reservation reservation);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsByRoomId(int roomId);
        Task<IEnumerable<Reservation>> GetReservationsByAccountId(int accountId);
        Task<IEnumerable<Reservation>> GetReservationsByPeriod(DateTime start, DateTime end);
        Task<IEnumerable<Reservation>> GetReservationsBySeriesId(int seriesId);
        Task<bool> IsRoomAvailable(int roomId, DateTime start, DateTime end);
    }
}
