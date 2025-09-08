using RR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation> AddAsync(Reservation student);
        Task<Reservation> GetReservationById(int id);
        Task<Reservation> UpdateAsync(Reservation servant);
        Task<Reservation> ApproveReservation(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Reservation>> GetReservationsByRoomId(int roomId);
        Task<IEnumerable<Reservation>> GetReservationsByAccountId(int accountId);
        Task<IEnumerable<Reservation>> GetReservationsByPeriod(DateTime start, DateTime end);
        Task<bool> CancelReservation(int id);
        Task<bool> IsRoomAvailable(int roomId, DateTime start, DateTime end);
        Task<IEnumerable<Reservation>> GetReservationsBySeriesId(int seriesId);
    }
}
