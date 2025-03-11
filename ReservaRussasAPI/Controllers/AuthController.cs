using Domain.Enums;
using Domain.Models;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RR.Core.DTOs;
using RR.Core.Services;
using RR.Service.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReservaRussasAPI.Controllers
{
    public class AuthController:ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<Account>>> Login(string userName, string senha)
        {
            var account = await _authService.Login(userName, senha);
            if (account != null)
            {
                var token = GenerateJwtToken(account);
                return Ok(new ApiResponse<string>(token, true, "Login efetuado com sucesso"));
            }
            return Unauthorized(new ApiResponse<Account>(null, false, "Usuário não encontrado"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> Register (AccountDTO accountDTO)
        {
            bool isSuccess = await _authService.Register(accountDTO);
            if (!isSuccess)
                return Unauthorized(new ApiResponse<bool>(isSuccess, isSuccess, "Não foi possível registrar conta"));

            return Ok(new ApiResponse<bool>(isSuccess, isSuccess, "Conta registrada com sucesso"));
        }

        private string GenerateJwtToken(Account account)
        {
            EAccountPermission accountPermission = (EAccountPermission)account.AccountPermission;
            var role = accountPermission.ToString();

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
               new Claim(ClaimTypes.Name, account.UserName),
               new Claim(ClaimTypes.Email, account.Mail),
               new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
