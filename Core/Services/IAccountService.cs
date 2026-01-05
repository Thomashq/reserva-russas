using RR.Core.Entities;
using RR.Core.Responses.Account;
namespace Core.Services
{
    public interface IAccountService
    {
        Task<bool> AddAsync(Account account);
        Task<Account> GetByIdAsync(int id);
        Task<Account> UpdateAsync(Account account);
        Task<Account> GetByUserIdAsync(string id);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> SetActiveStatusAsync(int id, bool isActive);
        Task<bool> DeleteAsync(int id);
        Task<List<AccountLookupResponse>> SearchAsync(string q, int? permission, int take);
    }
}
