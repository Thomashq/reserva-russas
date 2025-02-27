using Core.Repositories;
using Core.Services;
using Domain.Models;
using RR.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service.Service
{
    public class AccountService:IAccountService
    {
        private readonly IRepository<Account> _repository;

        public AccountService(IRepository<Account> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AccountDTO>> GetAllAsync()
        {
            var accounts = await _repository.GetAllAsync();
            return accounts.Select(a => new AccountDTO { Id = a.Id, UserName = a.UserName, Email = a.Mail, Phone = a.Phone });
        }

        public async Task<AccountDTO?> GetByIdAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id);
            return account != null ? new AccountDTO { Id = account.Id, UserName = account.UserName, Email = account.Mail, Phone = account.Phone } : null;
        }

        public async Task<AccountDTO> CreateAsync(AccountDTO dto)
        {
            var account = new Account { Id = dto.Id, UserName = dto.UserName, Mail = dto.Email, Phone = dto.Phone, PasswordHash = dto.Password };
            await _repository.AddAsync(account);
            return dto;
        }

        public async Task<AccountDTO?> UpdateAsync(Guid id, AccountDTO dto)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null) return null;

            account.UserName = dto.UserName;
            account.Mail = dto.Email;
            account.Phone = dto.Phone;
            account.PasswordHash = dto.Password;
            await _repository.UpdateAsync(account);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
