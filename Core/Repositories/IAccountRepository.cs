using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RR.Core.Entities;
using RR.Core.Repositories.Base;

namespace RR.Core.Repositories
{
    /// <summary>
    /// Interface específica para repositório de Account
    /// </summary>
    public interface IAccountRepository : IBaseRepository<Account, int>
    {
        /// <summary>
        /// Obtém uma conta pelo nome de usuário
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <returns>Conta encontrada ou null</returns>
        Task<Account?> GetByUserNameAsync(string userName);

        /// <summary>
        /// Obtém uma conta pelo email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Conta encontrada ou null</returns>
        Task<Account?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica se um nome de usuário está disponível
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <returns>True se disponível, false caso contrário</returns>
        Task<bool> IsUserNameAvailableAsync(string userName);

        /// <summary>
        /// Verifica se um email está disponível
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>True se disponível, false caso contrário</returns>
        Task<bool> IsEmailAvailableAsync(string email);

        /// <summary>
        /// Obtém contas ativas
        /// </summary>
        /// <returns>Lista de contas ativas</returns>
        Task<IEnumerable<Account>> GetActiveAccountsAsync();

        /// <summary>
        /// Obtém contas paginadas
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista paginada de contas</returns>
        Task<IEnumerable<Account>> GetPagedAccountsAsync(int page, int pageSize);

        /// <summary>
        /// Conta o total de contas
        /// </summary>
        /// <returns>Número total de contas</returns>
        Task<int> GetTotalAccountsCountAsync();

        /// <summary>
        /// Obtém contas por nome de usuário (busca parcial)
        /// </summary>
        /// <param name="userName">Nome de usuário ou parte dele</param>
        /// <returns>Lista de contas encontradas</returns>
        Task<IEnumerable<Account>> GetAccountsByUserNameAsync(string userName);
    }
}
