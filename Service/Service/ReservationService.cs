using Core.Repositories;
using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _repository;

        public ReservationService(IRepository<Reservation> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            return reservations.Select(r => new ReservationDTO { Id = r.Id, RoomId = r.RoomId, AccountId = r.AccountId, StartTime = r.StartTime, EndTime = r.EndTime });
        }

        public async Task<ReservationDTO?> GetByIdAsync(Guid id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            return reservation != null ? new ReservationDTO { Id = reservation.Id, RoomId = reservation.RoomId, AccountId = reservation.AccountId, StartTime = reservation.StartTime, EndTime = reservation.EndTime } : null;
        }

        public async Task<ReservationDTO> CreateAsync(ReservationDTO dto)
        {
            var reservation = new Reservation { Id = dto.Id, RoomId = dto.RoomId, AccountId = dto.AccountId, StartTime = dto.StartTime, EndTime = dto.EndTime };
            await _repository.AddAsync(reservation);
            return dto;
        }

        public async Task<ReservationDTO?> UpdateAsync(Guid id, ReservationDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.RoomId = dto.RoomId;
            existing.AccountId = dto.AccountId;
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;
            await _repository.UpdateAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
