using System.ComponentModel.DataAnnotations;

namespace RR.Core.DTOs.Requests
{
    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de refresh token
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de alteração de senha
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmação da nova senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "Senhas não conferem")]
        public string ConfirmNewPassword { get; set; }
    }

}
