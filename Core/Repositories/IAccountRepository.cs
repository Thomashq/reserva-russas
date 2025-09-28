using RR.Core.Entities;

namespace RR.Core.Repositories
{
    public interface IAccountRepository 
    {
        Task<bool> AddAsync(Account account);
        Task<Account> GetByIdAsync(int id);
        Task<Account> GetByUserIdAsync(string id);
        Task<Account?> GetByUserNameAsync(string userName);
        Task<Account?> UpdateAsync(Account account);
        Task<Account?> GetByEmailAsync(string email);
        Task<bool> IsUserNameAvailableAsync(string userName);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<IEnumerable<Account>> GetActiveAccountsAsync();
        Task<IEnumerable<Account>> GetPagedAccountsAsync(int page, int pageSize);
        Task<int> GetTotalAccountsCountAsync();
        Task<IEnumerable<Account>> GetAccountsByUserNameAsync(string userName);
        Task<bool> DeleteAsync(int id);
    }
}
