
using RR.Core.Entities;
using RR.Core.Services.Base;

namespace Core.Services
{
    public interface IAccountService:IBaseService<Account, int>
    {
        /// <summary>
        /// Obtém um usuário pelo email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Usuário encontrado ou null</returns>
        Task<Account?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtém um usuário pelo nome
        /// </summary>
        /// <param name="username">Username do usuário</param>
        /// <returns>Usuário encontrado ou null</returns>
        Task<Account?> GetByUsernameAsync(string username);

        /// <summary>
        /// Verifica se um email já existe no sistema
        /// </summary>
        /// <param name="email">Email a ser verificado</param>
        /// <returns>True se o email existir</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Ativa ou desativa um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="isActive">Status ativo</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        Task<bool> SetActiveStatusAsync(int id, bool isActive);
    }
}
