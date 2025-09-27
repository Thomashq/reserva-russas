
using RR.Core.Entities;
using RR.Core.Services.Base;

namespace Core.Services
{
    public interface IAccountService
    {
        Task<bool> AddAsync(Account account);
        Task<Account> GetByIdAsync(int id);
        Task<Account> UpdateAsync(Account account);
        Task<Account> GetByUserIdAsync(int id);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> SetActiveStatusAsync(int id, bool isActive);
        Task<bool> DeleteAsync(int id);

    }
}
