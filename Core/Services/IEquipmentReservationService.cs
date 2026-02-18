using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IEquipmentReservationService
    {
        Task<IEnumerable<EquipmentReservation>> GetAllAsync();
        Task<EquipmentReservation?> GetReservationById(int id);
        Task<EquipmentReservation> AddAsync(EquipmentReservation reservation);
        Task<EquipmentReservation?> UpdateAsync(EquipmentReservation reservation);
        Task<bool> DeleteAsync(int id);
        Task<EquipmentReservation?> ApproveReservation(int id);
        Task<bool> CancelReservation(int id);
        Task<IEnumerable<EquipmentReservation>> GetReservationsByEquipmentId(int equipmentId);
        Task<IEnumerable<EquipmentReservation>> GetReservationsByAccountId(int accountId);
        Task<IEnumerable<EquipmentReservation>> GetReservationsByPeriod(DateTime start, DateTime end);
        Task<bool> IsEquipmentAvailable(int equipmentId, DateTime start, DateTime end, int? excludeReservationId = null);
    }
}
