// ====================
// REQUESTS
// ====================

using System.ComponentModel.DataAnnotations;

namespace RR.Core.Requests.Account
{
    /// <summary>
    /// Request para criação de conta
    /// </summary>
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 100 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Senha deve ter pelo menos 8 caracteres")]
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
        public string Mail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Permissão deve ser um valor válido")]
        public int AccountPermission { get; set; } = 0;
    }

    /// <summary>
    /// Request para atualização de conta
    /// </summary>
    public class UpdateAccountRequest
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 100 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
        public string Mail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Permissão deve ser um valor válido")]
        public int AccountPermission { get; set; }
    }
}