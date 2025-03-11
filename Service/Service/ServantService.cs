using Core.Repositories;
using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service.Service
{
    public class ServantService:IServantService
    {
        private readonly IRepository<Servant> _repository;

        public ServantService(IRepository<Servant> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ServantDTO>> GetAllAsync()
        {
            var servants = await _repository.GetAllAsync();
            return servants.Select(s => new ServantDTO { Id = s.Id, AccountId = s.AccountId, Advisee = s.Advisee, Reservation = s.Reservation });
        }

        public async Task<ServantDTO?> GetByIdAsync(Guid id)
        {
            var servant = await _repository.GetByIdAsync(id);
            return servant != null ? new ServantDTO { Id = servant.Id, AccountId = servant.AccountId, Advisee = servant.Advisee, Reservation = servant.Reservation } : null;
        }

        public async Task<ServantDTO> CreateAsync(ServantDTO dto)
        {
            var servant = new Servant {AccountId = dto.AccountId, Advisee = dto.Advisee, Reservation = dto.Reservation };
            servant.CreationDate = DateTime.UtcNow;
            await _repository.AddAsync(servant);
            return dto;
        }

        public async Task<ServantDTO?> UpdateAsync(Guid id, ServantDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Advisee = dto.Advisee;
            existing.Reservation = dto.Reservation;
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
