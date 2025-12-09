using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RR.Core.Entities;              // Account, AppUsers
using RR.Core.Enums;                 // EAccountPermission
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Seeding
{
    public class SeedAdminOptions
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var config = sp.GetRequiredService<IConfiguration>();

            var options = new SeedAdminOptions
            {
                Email    = config["SeedAdmin:Email"],
                UserName = config["SeedAdmin:UserName"],
                Password = config["SeedAdmin:Password"],
            };

            if (string.IsNullOrWhiteSpace(options.Email) ||
                string.IsNullOrWhiteSpace(options.Password))
            {
                return;
            }

            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var db          = sp.GetRequiredService<ApplicationDbContext>();

            // 1) Garante o usuário Identity
            var user = await userManager.FindByEmailAsync(options.Email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName       = options.UserName,
                    Email          = options.Email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, options.Password);
                if (!createResult.Succeeded)
                    return;
            }

            var account = db.Account.FirstOrDefault(a => a.UserId == user.Id);
            if (account is null)
            {
                account = new Account
                {
                    UserId            = user.Id,
                    UserName          = options.UserName ?? user.UserName!,
                    Mail              = options.Email!,
                    AccountPermission = (int)EAccountPermission.Admin,
                    IsActive          = true
                };

                db.Account.Add(account);
            }
            else
            {
                account.AccountPermission = (int)EAccountPermission.Admin;
                account.IsActive          = true;
            }

            await db.SaveChangesAsync();

            var claims = await userManager.GetClaimsAsync(user);
            if (!claims.Any(c => c.Type == ClaimTypes.Role &&
                                 c.Value == EAccountPermission.Admin.ToString()))
            {
                await userManager.AddClaimAsync(
                    user,
                    new Claim(ClaimTypes.Role, EAccountPermission.Admin.ToString())
                );
            }
        }
    }
}

