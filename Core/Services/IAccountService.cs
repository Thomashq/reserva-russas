using Domain.Shared;
using RR.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDTO>> GetAllAsync();
        Task<AccountDTO?> GetByIdAsync(Guid id);
        Task<AccountDTO> CreateAsync(AccountDTO dto);
        Task<AccountDTO?> UpdateAsync(Guid id, AccountDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
