using Domain.Models;

namespace RR.Core.Repositories
{
    public interface IAuthRepository
    {
        Task <Account> ValidateUser(string Login, string Password);
        Task<bool> RegisterUser(Account account);
    }
}
