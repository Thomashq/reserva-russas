using Microsoft.EntityFrameworkCore;
using Core.Services;
using RR.Core.Entities;
using RR.Infraestructure.DataContext;

namespace RR.Service
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context) { _context = context; }

        public async Task<Account?> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;
            return await _context.Account.FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            email = email.Trim().ToLower();
            return await _context.Account.FirstOrDefaultAsync(a => a.Mail.ToLower() == email && a.IsActive);
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            username = username.Trim().ToLower();
            return await _context.Account.FirstOrDefaultAsync(a => a.UserName.ToLower() == username && a.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim().ToLower();
            return await _context.Account.AnyAsync(a => a.Mail.ToLower() == email && a.IsActive);
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Account.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> AddAsync(Account account)
        {
            account.Mail = account.Mail.ToLower();
            account.UserName = account.UserName.ToLower();
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;

            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Account> UpdateAsync(Account updatedAccount)
        {
            var existingAccount = await _context.Account.FirstOrDefaultAsync(a => a.Id == updatedAccount.Id);
            if (existingAccount == null) return null;

            existingAccount.UserName = updatedAccount.UserName?.ToLower();
            existingAccount.Mail = updatedAccount.Mail?.ToLower();
            existingAccount.Phone = updatedAccount.Phone;
            existingAccount.AccountPermission = updatedAccount.AccountPermission;
            existingAccount.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) return false;

            account.IsActive = isActive;
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
