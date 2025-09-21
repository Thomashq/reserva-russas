using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReservaRussasAPI.Controllers.Base; 
using RR.Core.DTOs.Requests;
using RR.Core.DTOs.Responses;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Responses.Account;
using RR.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ReservaRussasAPI.Controllers
{
    public class AuthController : BaseControllerFYP
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IAccountService accountService, IConfiguration configuration)
        {
            _authService = authService;
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var account = await _authService.Login(request.UserName, request.Password);
            if (account is null) return ResponseUnauthorized("Usuário ou senha inválidos");

            string token = GenerateJwtToken(account.UserId, account);

            return ResponseOk(token);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
        {
            var ok = await _authService.Register(request);
            if (!ok) return ResponseBadRequest("Não foi possível registrar a conta");

            var resp = new AccountCreatedResponse
            {
                UserName = request.UserName,
                Mail = request.Mail,
                Phone = request.Phone,
                // AccountPermission agora é definido pelo email no service
            };
            return ResponseCreated(resp);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out _);
            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return ResponseUnauthorized("Token inválido");

            var account = await _accountService.GetByUserIdAsync(userId);
            if (account is null) return ResponseUnauthorized("Usuário não encontrado");

            var newToken = GenerateJwtToken(userId, account);
            var refreshResponse = new RefreshTokenResponse
            {
                Token = newToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return ResponseOk(refreshResponse);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return ResponseNotFound("Não autenticado");

            var account = await _accountService.GetByUserIdAsync(userId);
            if (account is null) return ResponseNotFound("Usuário não encontrado");

            var dto = new AccountResponse
            {
                Id = account.Id,
                UserName = account.UserName,
                Mail = account.Mail,
                Phone = account.Phone,
                AccountPermission = account.AccountPermission,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
            return ResponseOk(dto);
        }

        private string GenerateJwtToken(int userId, Account account)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, account.UserName),
            new Claim(ClaimTypes.Email, account.Mail),
            new Claim("permission", account.AccountPermission.ToString())
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
