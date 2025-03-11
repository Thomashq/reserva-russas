using Domain.Models;
using RR.Core.DTOs;

namespace RR.Core.Services
{
    public interface IAuthService
    {
        Task<Account> Login(string login, string senha);

        Task<bool> Logout();

        Task<bool> Register(AccountDTO dto);
    }
}
