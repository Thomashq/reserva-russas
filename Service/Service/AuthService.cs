using Microsoft.AspNetCore.Identity;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Requests.Account;
using RR.Core.Services;
using RR.Util.Criptography; // HashPass

namespace RR.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IServantRepository _servantRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly HashPass _hash = new(); // stateless

        public AuthService(
            UserManager<AppUser> userManager,
            IServantRepository servantRepository,
            IStudentRepository studentRepository,
            IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _servantRepository = servantRepository;
            _studentRepository = studentRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Account?> Login(string login, string senha)
        {
            // aceita username OU email
            var user = await _userManager.FindByNameAsync(login)
                       ?? await _userManager.FindByEmailAsync(login);

            if (user is null || !user.IsActive) return null;

            var ok = await _userManager.CheckPasswordAsync(user, senha);
            if (!ok) return null;

            return await _accountRepository.GetByIdAsync(user.Id);
        }

        public Task<bool> Logout()
        {
            return Task.FromResult(true);
        }

        public async Task<bool> Register(CreateAccountRequest dto)
        {
            // checagens básicas no Identity
            if (await _userManager.FindByNameAsync(dto.UserName) is not null) return false;
            if (await _userManager.FindByEmailAsync(dto.Mail) is not null) return false;

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Mail,
                PhoneNumber = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var res = await _userManager.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return false;

            var account = new Account
            {
                UserId = user.Id, 
                UserName = dto.UserName,
                Mail = dto.Mail,
                Phone = dto.Phone,
                AccountPermission = dto.AccountPermission,
                PasswordHash = _hash.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _accountRepository.AddAsync(account);
            if (!ok) return false;

            // cria o “perfil” (Servant/Student) conforme AccountPermission
            return await CreateAccountProfile(account);
        }

        public async Task<bool> CreateAccountProfile(Account account)
        {
            switch (account.AccountPermission)
            {
                case 1:
                    {
                        var servant = new Servant { AccountId = account.Id /*, Account = account */ };
                        return await _servantRepository.AddAsync(servant);
                    }
                case 2:
                    {
                        var student = new Student { AccountId = account.Id /*, Account = account */ };
                        return await _studentRepository.AddAsync(student);
                    }
                default:
                    return true; 
            }
        }

    }
}
