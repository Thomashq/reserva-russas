using Core.Services;
using RR.Core.Entities;
using RR.Core.Repositories;


namespace RR.Service
{
    public class AccountService: IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository) 
        { 
            _accountRepository = accountRepository;
        }

        public async Task<bool> AddAsync(Account account)
        {
            if (account == null)
                return false;

            // Validações básicas
            if (string.IsNullOrWhiteSpace(account.Mail) || string.IsNullOrWhiteSpace(account.UserName))
                return false;

            // Verifica se email já existe
            if (await EmailExistsAsync(account.Mail))
                return false;

            // Verifica se username já existe
            if (await GetByUsernameAsync(account.UserName) != null)
                return false;

            // Normaliza dados
            account.Mail = account.Mail.ToLower();
            account.UserName = account.UserName.ToLower();
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;

            return await _accountRepository.AddAsync(account);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _accountRepository.GetByEmailAsync(email.ToLower());
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _accountRepository.GetByUserNameAsync(username.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await GetByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            //await UpdateAsync(id, user);
            return true;
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<Account> UpdateAsync(Account account)
        {
            return await _accountRepository.UpdateAsync(account);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _accountRepository.DeleteAsync(id);
        }
    }
}
