using RR.Core.DTOs;

namespace RR.Core.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllAsync();
        Task<ReservationDTO?> GetByIdAsync(Guid id);
        Task<ReservationDTO> CreateAsync(ReservationDTO dto);
        Task<ReservationDTO?> UpdateAsync(Guid id, ReservationDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
