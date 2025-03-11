using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Repositories;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository) 
        {
            _authRepository = authRepository;
        }

        public async Task<Account> Login(string login, string senha)
        {
            var account = await _authRepository.ValidateUser(login, senha);
            
            if (account != null) 
                return account;
            
            return null;
        }

        public Task<bool> Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Register(AccountDTO dto)
        {
            var createdAccount = new Account
            {
                Mail = dto.Email,
                UserName = dto.UserName,
                PasswordHash = dto.Password,
                Phone = dto.Phone,
                AccountPermission = dto.AccountPermission
            };
            bool IsSuccess = await _authRepository.RegisterUser(createdAccount);

            return IsSuccess;
        }
    }
}
