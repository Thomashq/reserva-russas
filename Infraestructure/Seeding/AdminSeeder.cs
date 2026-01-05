using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Seeding;

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
            return;

        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var db          = sp.GetRequiredService<ApplicationDbContext>();

        // 1) garantir roles no banco
        string[] roles = { "Admin", "Manager", "Servant", "Student" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2) garantir usuário admin
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

        // 3) garantir account
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

        // 4) garantir que o usuário está na role Admin
        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
        //garantir o seeding de managers para ter contas que obedeçam a regra de negócio de fK, sem workaround

    }

    public class SeedAdminOptions
    {
      public string? Email { get; set; }
      public string? UserName { get; set; }
      public string? Password { get; set; }
    }
}

