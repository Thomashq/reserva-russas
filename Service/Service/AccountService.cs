using Core.Services;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Core.Services.Base;


namespace RR.Service
{
    public class AccountService : BaseService<Account, int>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository) : base(accountRepository) 
        { 
            _accountRepository = accountRepository;
        }
        /// <summary>
        /// Obtém um usuário pelo email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Usuário encontrado ou null</returns>
        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _accountRepository.FirstOrDefaultAsync(u => u.Mail.ToLower() == email.ToLower());
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _accountRepository.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        /// <summary>
        /// Verifica se um email já existe no sistema
        /// </summary>
        /// <param name="email">Email a ser verificado</param>
        /// <returns>True se o email existir</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await GetByEmailAsync(email);
            return user != null;
        }

        /// <summary>
        /// Ativa ou desativa um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="isActive">Status ativo</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(id, user);
            return true;
        }

        /// <summary>
        /// Validações específicas para criação de usuário
        /// </summary>
        /// <param name="entity">Usuário a ser criado</param>
        protected override async Task ValidateForCreate(Account entity)
        {
            if (string.IsNullOrWhiteSpace(entity.UserName))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(entity.Mail))
                throw new ArgumentException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(entity.PasswordHash))
                throw new ArgumentException("Senha é obrigatória");

            // Verifica se o email já existe
            var emailExists = await EmailExistsAsync(entity.Mail);
            if (emailExists)
                throw new InvalidOperationException("Email já está em uso");

            await base.ValidateForCreate(entity);
        }

        /// <summary>
        /// Validações específicas para atualização de usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="entity">Dados atualizados</param>
        protected override async Task ValidateForUpdate(int id, Account entity)
        {
            if (string.IsNullOrWhiteSpace(entity.UserName))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(entity.Mail))
                throw new ArgumentException("Email é obrigatório");

            // Verifica se o email já está em uso por outro usuário
            var existingUserWithEmail = await GetByEmailAsync(entity.Mail);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
                throw new InvalidOperationException("Email já está em uso por outro usuário");

            await base.ValidateForUpdate(id, entity);
        }

        /// <summary>
        /// Validações específicas para exclusão de usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        protected override async Task ValidateForDelete(int id)
        {
            // Adicione validações específicas se necessário
            // Por exemplo: verificar se o usuário tem dependências

            await base.ValidateForDelete(id);
        }

        /// <summary>
        /// Atualiza propriedades específicas da entidade
        /// </summary>
        /// <param name="existingEntity">Entidade existente</param>
        /// <param name="newEntity">Novos dados</param>
        protected override void UpdateEntity(Account existingEntity, Account newEntity)
        {
            existingEntity.UserName = newEntity.UserName;
            existingEntity.Mail = newEntity.Mail;
            existingEntity.IsActive = newEntity.IsActive;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            // Só atualiza a senha se uma nova foi fornecida
            if (!string.IsNullOrWhiteSpace(newEntity.PasswordHash))
            {
                existingEntity.PasswordHash = newEntity.PasswordHash;
            }
        }
    }
}
