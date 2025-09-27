using RR.Core.Responses.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs.Responses
{
    /// <summary>
    /// DTO para resposta de login
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public AccountResponse Account { get; set; } = new();
    }
    /// <summary>
    /// DTO para resposta de refresh token
    /// </summary>
    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
