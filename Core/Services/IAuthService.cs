using RR.Core.Entities;
using RR.Core.Requests.Account;

namespace RR.Core.Services
{
    public interface IAuthService
    {
        Task<Account> Login(string login, string senha);

        Task<bool> Logout();

        Task<bool> Register(CreateAccountRequest dto);
    }
}
