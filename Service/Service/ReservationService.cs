// RR.Service/Service/ReservationService.cs
using Microsoft.Extensions.Logging;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservations;
        private readonly ILogger<ReservationService>? _logger;

        public ReservationService(IReservationRepository reservations, ILogger<ReservationService>? logger = null)
        {
            _reservations = reservations;
            _logger = logger;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
            => await _reservations.GetAllAsync();

        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            if (reservation is null) throw new ArgumentNullException(nameof(reservation));
            if (reservation.RoomId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.RoomId));
            if (reservation.AccountId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.AccountId));
            if (reservation.StartTime >= reservation.EndTime) throw new ArgumentException("StartTime must be before EndTime.");

            var available = await _reservations.IsRoomAvailable(reservation.RoomId, reservation.StartTime, reservation.EndTime);
            
            if (!available)
                throw new InvalidOperationException("Room is not available for the selected period.");

            reservation.Status = 0; // Criado
            
            var created = await _reservations.AddAsync(reservation);
            _logger?.LogInformation("Reservation created: {ReservationId} Room {RoomId} ({Start} - {End})",
                created.Id, created.RoomId, created.StartTime, created.EndTime);

            return created;
        }

        public async Task<Reservation> ApproveReservation(int id)
        {
            var reservation = await _reservations.GetReservationById(id);
            reservation.Status = 1; // Aprovado

            var updated = await _reservations.UpdateAsync(reservation);

            if (updated is null)
            {
                _logger?.LogError("Failed to approve reservation {ReservationId}", id);
                return null!;
            }

            _logger?.LogInformation("Reservation approved: {ReservationId}", id);
            return updated;
        }
        public async Task<Reservation> GetReservationById(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return await _reservations.GetReservationById(id);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByRoomId(int roomId)
        {
            if (roomId <= 0) throw new ArgumentOutOfRangeException(nameof(roomId));
            return await _reservations.GetReservationsByRoomId(roomId);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByAccountId(int accountId)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            return await _reservations.GetReservationsByAccountId(accountId);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByPeriod(DateTime start, DateTime end)
        {
            if (start >= end) throw new ArgumentException("Start must be before End.");
            return await _reservations.GetReservationsByPeriod(start, end);
        }

        public async Task<bool> IsRoomAvailable(int roomId, DateTime start, DateTime end)
        {
            if (roomId <= 0) throw new ArgumentOutOfRangeException(nameof(roomId));
            if (start >= end) throw new ArgumentException("Start must be before End.");
            return await _reservations.IsRoomAvailable(roomId, start, end);
        }

        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            if (reservation is null) throw new ArgumentNullException(nameof(reservation));
            if (reservation.Id <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.Id));
            if (reservation.RoomId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.RoomId));
            if (reservation.AccountId <= 0) throw new ArgumentOutOfRangeException(nameof(reservation.AccountId));
            if (reservation.StartTime >= reservation.EndTime) throw new ArgumentException("StartTime must be before EndTime.");

            // Evitar conflito com outras reservas (ignora a própria)
            var existingInRoom = await _reservations.GetReservationsByRoomId(reservation.RoomId);
            var hasConflict = existingInRoom.Any(r =>
                r.IsActive &&
                r.Id != reservation.Id &&
                r.StartTime < reservation.EndTime &&
                r.EndTime > reservation.StartTime);

            if (hasConflict)
                throw new InvalidOperationException("Room is not available for the selected period.");

            var updated = await _reservations.UpdateAsync(reservation);
            if (updated is null)
            {
                _logger?.LogWarning("Attempt to update non-existent reservation {ReservationId}", reservation.Id);
                return null!;
            }

            _logger?.LogInformation("Reservation updated: {ReservationId} Room {RoomId} ({Start} - {End})",
                updated.Id, updated.RoomId, updated.StartTime, updated.EndTime);

            return updated;
        }

        public async Task<bool> CancelReservation(int id) {             
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            
            var reservation = await _reservations.GetReservationById(id);
            if (reservation is null || !reservation.IsActive)
            {
                _logger?.LogWarning("Attempt to cancel non-existent or inactive reservation {ReservationId}", id);
                return false;
            }
            
            reservation.Status = 2; // Rejeitado/cancelado
            
            var updated = await _reservations.UpdateAsync(reservation);
            if (updated is null)
            {
                _logger?.LogError("Failed to cancel reservation {ReservationId}", id);
                return false;
            }
            _logger?.LogInformation("Reservation cancelled: {ReservationId}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            var ok = await _reservations.DeleteAsync(id);
            if (ok)
                _logger?.LogInformation("Reservation deleted (soft): {ReservationId}", id);
            else
                _logger?.LogWarning("Attempt to delete non-existent reservation {ReservationId}", id);
            return ok;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsBySeriesId(int seriesId)
        {
            if (seriesId <= 0) throw new ArgumentOutOfRangeException(nameof(seriesId));
            return await _reservations.GetReservationsBySeriesId(seriesId);
        }
    }
}
