using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Core.Repositories;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class AuthRepository:IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IServantRepository _servantRepository;
        private readonly IPasswordService _passwordService;
        public AuthRepository(ApplicationDbContext context, IPasswordService passwordService, IStudentRepository studentRepository, IServantRepository servantRepository)
        {
            _context = context;
            _passwordService = passwordService;
            _studentRepository = studentRepository;
            _servantRepository = servantRepository;
        }   

        public async Task<bool> RegisterUser(Account account)
        {
            var uniqueAccount = _context.Account.FirstOrDefault(x => x.Mail == account.Mail);

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
                    var studentAccount = new Student{
                        AccountId = account.Id
                    };

                    await _studentRepository.AddAsync(studentAccount);
                break;
                
                case EAccountPermission.Servant:
                    var servantAccount = new Servant 
                    {
                        AccountId = account.Id
                    };
                    await _servantRepository.AddAsync(servantAccount);
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
