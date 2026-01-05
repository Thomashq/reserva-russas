using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Seeding;

public static class ManagerSeeder
{
    public static async Task SeedManagersAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();

        if (db.Manager.Any()) return;

        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

        // garantir role Manager
        if (!await roleManager.RoleExistsAsync("Manager"))
            await roleManager.CreateAsync(new IdentityRole("Manager"));

        var managers = ManagerList.ReturnManagerList();

        foreach (var m in managers)
        {
            var user = await userManager.FindByEmailAsync(m.Email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = m.UserName,
                    Email = m.Email,
                    EmailConfirmed = true,
                    FullName = m.FullName,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, m.Password);
                if (!createResult.Succeeded) continue;
            }

            if (!await userManager.IsInRoleAsync(user, "Manager"))
                await userManager.AddToRoleAsync(user, "Manager");

            var account = db.Account.FirstOrDefault(a => a.UserId == user.Id);
            if (account is null)
            {
                account = new Account
                {
                    UserId = user.Id,
                    UserName = m.UserName ?? user.UserName!,
                    Mail = m.Email,
                    Phone = m.Phone,
                    AccountPermission = (int)EAccountPermission.Manager,
                    IsActive = true
                };

                db.Account.Add(account);
                await db.SaveChangesAsync(); // precisa do Id pra FK do Manager
            }
            else
            {
                account.AccountPermission = (int)EAccountPermission.Manager;
                account.IsActive = true;
                await db.SaveChangesAsync();
            }

            var manager = db.Manager.FirstOrDefault(x => x.AccountId == account.Id);
            if (manager is null)
            {
                manager = new Manager
                {
                    AccountId = account.Id,
                    Account = account,
                    IsActive = true
                };

                db.Manager.Add(manager);
                await db.SaveChangesAsync();
            }
        }
    }

    private static class ManagerList
    {
        public static List<SeedManagerItem> ReturnManagerList()
        {
            return new List<SeedManagerItem>
            {
                new SeedManagerItem { FullName = "Manager 01", UserName = "manager01", Email = "manager01@rr.local", Password = "12345678" },
                new SeedManagerItem { FullName = "Manager 02", UserName = "manager02", Email = "manager02@rr.local", Password = "12345678" },
                new SeedManagerItem { FullName = "Manager 03", UserName = "manager03", Email = "manager03@rr.local", Password = "12345678" },
            };
        }
    }

    private sealed class SeedManagerItem
    {
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Phone { get; set; }
    }
}

