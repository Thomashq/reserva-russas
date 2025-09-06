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
        private readonly HashPass _hash = new();

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
            var user = await _userManager.FindByNameAsync(login)
                       ?? await _userManager.FindByEmailAsync(login);
            if (user is null || !user.IsActive) return null;

            var ok = await _userManager.CheckPasswordAsync(user, senha);
            if (!ok) return null;

            return await _accountRepository.GetByUserIdAsync(user.Id);
        }

        public Task<bool> Logout() => Task.FromResult(true);

        public async Task<bool> Register(CreateAccountRequest dto)
        {
            if (await _userManager.FindByNameAsync(dto.UserName) is not null) return false;
            if (await _userManager.FindByEmailAsync(dto.Mail) is not null) return false;

            // regra de domínio
            if (!IsUfcEmail(dto.Mail)) return false;
            var resolvedPermission = ResolvePermissionFromEmail(dto.Mail); // 1=Servant, 2=Student, 0=Default

            var user = new AppUser
            {
                FullName = dto.FullName,
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
                AccountPermission = resolvedPermission,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _accountRepository.AddAsync(account);
            if (!ok) return false;

            return await CreateAccountProfile(account); // cria Servant/Student
        }

        public async Task<bool> CreateAccountProfile(Account account)
        {
            switch (account.AccountPermission)
            {
                case 1: // Servant
                    return await _servantRepository.AddAsync(new Servant { AccountId = account.Id });
                case 2: // Student
                    return await _studentRepository.AddAsync(new Student { AccountId = account.Id });
                default:
                    return true;
            }
        }

        // helpers de domínio
        private static bool IsUfcEmail(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail)) return false;
            var m = mail.Trim().ToLowerInvariant();
            return m.EndsWith("@ufc.br") || m.EndsWith("@alu.ufc.br");
        }

        private static int ResolvePermissionFromEmail(string mail)
        {
            var m = (mail ?? "").Trim().ToLowerInvariant();
            if (m.EndsWith("@alu.ufc.br")) return 2; // Student
            if (m.EndsWith("@ufc.br")) return 1; // Servant
            return 0;
        }
    }
}
