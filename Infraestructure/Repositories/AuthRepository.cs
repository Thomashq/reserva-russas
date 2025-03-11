using Domain.Enums;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using RR.Core.DTOs;
using RR.Core.Repositories;
using RR.Core.Services;

namespace RR.Infraestructure.Repositories
{
    public class AuthRepository:IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IStudentService _studentService;
        private readonly IServantService _servantService;
        private readonly IPasswordService _passwordService;
        public AuthRepository(DataContext context, IPasswordService passwordService, IStudentService studentService, IServantService servantService)
        {
            _context = context;
            _passwordService = passwordService;
            _studentService = studentService;
            _servantService = servantService;
        }   

        public async Task<bool> RegisterUser(Account account)
        {
            var uniqueAccount = _context.Account.FirstOrDefault(x => x.UserName == account.UserName);

            if (uniqueAccount != null) 
                return false;

            account.PasswordHash = _passwordService.HashPassword(account.PasswordHash);

            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();

            return await RegisterRole(account);
        }
        private async Task<bool> RegisterRole(Account account)
        {
            if (account == null)
                return false;

            EAccountPermission type = (EAccountPermission)account.AccountPermission;

            switch (type)
            {
                case EAccountPermission.Student:
                    var studentAccount = new StudentDTO{
                        AccountId = account.Id
                    };

                    await _studentService.CreateAsync(studentAccount);
                break;
                
                case EAccountPermission.Servant:
                    var servantAccount = new ServantDTO 
                    {
                        AccountId = account.Id
                    };
                    await _servantService.CreateAsync(servantAccount);
                break;

                default:
                    return false;
                    
            }
            return true;
        }

        public async Task<Account> ValidateUser(string login, string password)
        {
            var account = await _context.Account.FirstOrDefaultAsync(x => x.UserName == login);

            if (account != null)
            {
                bool isPasswordValid = _passwordService.VerifyPassword(account.PasswordHash, password);

                if (isPasswordValid)
                {
                    return account;
                }
            }

            return null;
        }
    }
}
