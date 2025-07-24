using global::RR.Core.Entities;
using global::RR.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using RR.Infraestructure.DataContext;

namespace RR.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Account?> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return await _context.Account
                .Where(a => !a.IsActive && a.UserName.ToLower() == userName.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Account
                .Where(a => !a.IsActive && a.Mail.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserNameAvailableAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            return !await _context.Account
                .Where(a => !a.IsActive)
                .AnyAsync(a => a.UserName.ToLower() == userName.ToLower());
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !await _context.Account
                .Where(a => !a.IsActive)
                .AnyAsync(a => a.Mail.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
        {
            return await _context.Account
                .Where(a => !a.IsActive && a.IsActive)
                .OrderBy(a => a.UserName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetPagedAccountsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return await _context.Account
                .Where(a => !a.IsActive)
                .OrderBy(a => a.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalAccountsCountAsync()
        {
            return await _context.Account
                .Where(a => !a.IsActive)
                .CountAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new List<Account>();

            return await _context.Account
                .Where(a => !a.IsActive && a.UserName.ToLower().Contains(userName.ToLower()))
                .OrderBy(a => a.UserName)
                .ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Account.
                Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Account?> UpdateAsync(Account updatedAccount)
        {
            var existingAccount = await _context.Account
                .Where(a => a.Id == updatedAccount.Id).FirstOrDefaultAsync();

            existingAccount.Mail = updatedAccount.Mail;
            existingAccount.UserName = updatedAccount.UserName;
            existingAccount.Phone = updatedAccount.Phone;
            existingAccount.UpdatedAt = DateTime.UtcNow;

            _context.Account.Update(existingAccount);
            await _context.SaveChangesAsync();

            return existingAccount;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var accountToDelete = await _context.Account.Where(a => a.Id == id).FirstOrDefaultAsync();
            accountToDelete.IsActive = false;

            return true;
        }

        public async Task<bool> AddAsync(Account account)
        {
            account.IsActive = true;
            _context.Account.AddAsync(account);

           await _context.SaveChangesAsync();

            return true;
        }
    }
}