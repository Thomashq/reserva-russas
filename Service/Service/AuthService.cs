using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Requests.Account;
using RR.Core.Services;

namespace RR.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IServantRepository _servantRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAccountRepository _accountRepository;

        public AuthService(IAuthRepository authRepository, IServantRepository servantRepository, IStudentRepository studentRepository, IAccountRepository accountRepository)
        {
            _authRepository = authRepository;
            _servantRepository = servantRepository;
            _studentRepository = studentRepository;
            _accountRepository = accountRepository;
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

        public async Task<bool> Register(CreateAccountRequest dto)
        {
            var createdAccount = new Account
            {
                Mail = dto.Mail,
                UserName = dto.UserName,
                PasswordHash = dto.Password,
                Phone = dto.Phone,
                AccountPermission = dto.AccountPermission
            };

            bool isAccountCreated = await _authRepository.RegisterUser(createdAccount);

            if (isAccountCreated)
            {
                var accountWithId = await _accountRepository.GetByEmailAsync(dto.Mail);

                if (accountWithId != null)
                {
                    var profileCreated = await CreateAccountProfile(accountWithId);
                    return profileCreated;
                }
            }

            return false;
        }

        public async Task<bool> CreateAccountProfile(Account account)
        {
            switch (account.AccountPermission)
            {
                case 0:
                    return false;

                case 1:
                    var servant = new Servant
                    {
                        AccountId = account.Id,
                        Account = account
                    };
                    var servantResult = await _servantRepository.AddAsync(servant);
                    return servantResult != null;

                case 2:
                    var student = new Student
                    {
                        AccountId = account.Id,
                        Account = account
                    };
                    var studentResult = await _studentRepository.AddAsync(student);
                    return studentResult != null;

                default:
                    return false;
            }
        }
    }
}