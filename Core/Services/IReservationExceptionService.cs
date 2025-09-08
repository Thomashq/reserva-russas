using RR.Core.DTOs.Requests;
using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IReservationExceptionService
    {
        Task<ReservationException> ApplyExceptionSkip(ApplyExceptionSkipRequest move);
        Task<ReservationException> ApplyExceptionMove(ApplyExceptionMoveRequest move);
        Task CancelException(int exceptionId);
        Task<List<ReservationException>> GetExceptionsBySeries(int seriesId);
    }
}
