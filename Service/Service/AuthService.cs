using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Services;
using RR.Infraestructure.DataContext;
using RR.Util.Criptography; // HashPass

namespace RR.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly HashPass _hash = new();

        public AuthService(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Account?> Login(string login, string senha)
        {
            var user = await _userManager.FindByNameAsync(login)
                       ?? await _userManager.FindByEmailAsync(login);
            if (user is null || !user.IsActive) return null;

            var ok = await _userManager.CheckPasswordAsync(user, senha);
            if (!ok) return null;

            return await _context.Account.FirstOrDefaultAsync(a => a.UserId == user.Id && a.IsActive);
        }

        public Task<bool> Logout() => Task.FromResult(true);

        public async Task<bool> Register(CreateAccountRequest dto)
        {
          try
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

            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();

            return await CreateAccountProfile(account, user); // cria Servant/Student
          }
          catch(Exception ex)
          {
            throw new Exception("Erro: ", ex);
          }
        }

        public async Task<bool> CreateAccountProfile(Account account, AppUser user)
        {
            switch (account.AccountPermission)
            {
                case 1: // Servant
                  if(!await _userManager.IsInRoleAsync(user, "Servant"))
                    await _userManager.AddToRoleAsync(user, "Servant");
                  await _context.Servant.AddAsync(new Servant { AccountId = account.Id, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                  await _context.SaveChangesAsync();
                  return true;
                case 2: // Student
                  if(!await _userManager.IsInRoleAsync(user, "Student"))
                   await _userManager.AddToRoleAsync(user, "Student");
                  await _context.Student.AddAsync(new Student { AccountId = account.Id, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                  await _context.SaveChangesAsync();
                  return true;
                default:
                  return true;
          }
      }

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
