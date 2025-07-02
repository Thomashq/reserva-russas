using global::RR.Core.Entities;
using global::RR.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using RR.Infraestructure.DataContext;
using RR.Infraestructure.Repositories.Base;

namespace RR.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de contas
    /// </summary>
    public class AccountRepository : BaseRepository<Account, int>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtém uma conta pelo nome de usuário
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <returns>Conta encontrada ou null</returns>
        public async Task<Account?> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return await _dbSet
                .Where(a => !a.IsActive && a.UserName.ToLower() == userName.ToLower())
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém uma conta pelo email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Conta encontrada ou null</returns>
        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _dbSet
                .Where(a => !a.IsActive && a.Mail.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Verifica se um nome de usuário está disponível
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <returns>True se disponível, false caso contrário</returns>
        public async Task<bool> IsUserNameAvailableAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            return !await _dbSet
                .Where(a => !a.IsActive)
                .AnyAsync(a => a.UserName.ToLower() == userName.ToLower());
        }

        /// <summary>
        /// Verifica se um email está disponível
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>True se disponível, false caso contrário</returns>
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !await _dbSet
                .Where(a => !a.IsActive)
                .AnyAsync(a => a.Mail.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Obtém contas ativas
        /// </summary>
        /// <returns>Lista de contas ativas</returns>
        public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
        {
            return await _dbSet
                .Where(a => !a.IsActive && a.IsActive)
                .OrderBy(a => a.UserName)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém contas paginadas
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista paginada de contas</returns>
        public async Task<IEnumerable<Account>> GetPagedAccountsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return await _dbSet
                .Where(a => !a.IsActive)
                .OrderBy(a => a.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Conta o total de contas
        /// </summary>
        /// <returns>Número total de contas</returns>
        public async Task<int> GetTotalAccountsCountAsync()
        {
            return await _dbSet
                .Where(a => !a.IsActive)
                .CountAsync();
        }

        /// <summary>
        /// Obtém contas por nome de usuário (busca parcial)
        /// </summary>
        /// <param name="userName">Nome de usuário ou parte dele</param>
        /// <returns>Lista de contas encontradas</returns>
        public async Task<IEnumerable<Account>> GetAccountsByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new List<Account>();

            return await _dbSet
                .Where(a => !a.IsActive && a.UserName.ToLower().Contains(userName.ToLower()))
                .OrderBy(a => a.UserName)
                .ToListAsync();
        }

        /// <summary>
        /// Override do método GetAllAsync para ordenar por nome de usuário e filtrar deletados
        /// </summary>
        /// <returns>Lista de todas as contas não deletadas ordenadas por nome de usuário</returns>
        public override async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _dbSet
                .Where(a => !a.IsActive)
                .OrderBy(a => a.UserName)
                .ToListAsync();
        }

        /// <summary>
        /// Override do método GetByIdAsync para filtrar deletados
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Conta encontrada ou null</returns>
        public override async Task<Account?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(a => !a.IsActive && a.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Override do método DeleteAsync para fazer soft delete
        /// </summary>
        /// <param name="id">ID da conta</param>
        public override async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                // Soft delete
                entity.IsActive = true;
                _dbSet.Update(entity);
            }
        }

        /// <summary>
        /// Override do método ExistsAsync para filtrar deletados
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>True se existe e não está deletada</returns>
        public override async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet
                .AnyAsync(a => !a.IsActive && a.Id == id);
        }
    }
}