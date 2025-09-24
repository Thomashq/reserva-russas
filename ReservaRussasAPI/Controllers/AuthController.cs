using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            if (account is null)
                return ResponseUnauthorized("Usuário ou senha inválidos");
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim("account", JsonConvert.SerializeObject(account)),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("reserva-russas-token-auth-chave-autenticacao-token-jwt-handler-auto"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "reservas-russas",
                audience: "sistema",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // retorna só o token puro (string)
            return ResponseOk(tokenString);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
        {
            var ok = await _authService.Register(request);
            if (!ok)
                return ResponseBadRequest("Não foi possível registrar a conta");

            var resp = new
            {
                request.UserName,
                request.Mail,
                request.Phone
            };
            return ResponseCreated(resp);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest? request)
        {
            var tokenString = request?.Token;
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                tokenString = authHeader.StartsWith("Bearer ") ? authHeader[7..] : authHeader;
            }

            if (string.IsNullOrWhiteSpace(tokenString))
                return ResponseUnauthorized("Token não informado");

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(tokenString, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseUnauthorized("Falha ao tentar renovar o token");
                }

                var userIdStr = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                             ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
                    return ResponseUnauthorized("Token inválido");

                var account = await _accountService.GetByUserIdAsync(userId);
                if (account is null)
                    return ResponseUnauthorized("Usuário não encontrado");

                var newToken = GenerateJwtToken(account);
                // retorna só o token puro
                return ResponseOk(newToken);
            }
            catch
            {
                return ResponseUnauthorized("Falha ao tentar renovar o token");
            }
        }

        private string GenerateJwtToken(Account account)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim("account", JsonConvert.SerializeObject(account)),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
