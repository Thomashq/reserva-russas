using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservaRussasAPI.Controllers.Base; // suas helpers ResponseOk/BadRequest/etc
using RR.Core.DTOs.Requests;
using RR.Core.DTOs.Responses;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Responses.Account;
using RR.Infraestructure.DataContext;
using RR.Util.Criptography; // HashPass
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ReservaRussasAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : BaseControllerFYP
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly HashPass _hash = new(); // util PBKDF2 (stateless)

        public AuthController(
            UserManager<AppUser> userManager,
            ApplicationDbContext db,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
        }

        /// <summary>Realiza login do usuário</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var validation = ValidateModelState();
                if (validation is not null) return validation;

                var user = await _userManager.FindByNameAsync(request.UserName)
                           ?? await _userManager.FindByEmailAsync(request.UserName);

                if (user is null || !user.IsActive)
                    return ResponseUnauthorized("Usuário ou senha inválidos");

                var ok = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!ok) return ResponseUnauthorized("Usuário ou senha inválidos");

                var account = await _db.Account.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (account is null) return ResponseUnauthorized("Conta não encontrada");

                var token = GenerateJwtToken(user, account);

                var loginResponse = new LoginResponse
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(2),
                    Account = new AccountResponse
                    {
                        Id = account.Id,
                        UserName = account.UserName,
                        Mail = account.Mail,
                        Phone = account.Phone,
                        AccountPermission = account.AccountPermission,
                        IsActive = account.IsActive,
                        CreatedAt = account.CreatedAt,
                        UpdatedAt = account.UpdatedAt
                    }
                };

                return ResponseOk(loginResponse, "Login efetuado com sucesso");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>Registra uma nova conta</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
        {
            try
            {
                var validation = ValidateModelState();
                if (validation is not null) return validation;

                if (!Regex.IsMatch(request.Mail ?? "",
                    @"^[^@]+@(ufc\.br|alu\.ufc\.br)$",
                    RegexOptions.IgnoreCase))
                {
                    return ResponseBadRequest("O e-mail deve ser @ufc.br ou @alu.ufc.br");
                }
                if (await _userManager.FindByNameAsync(request.UserName) is not null)
                    return ResponseBadRequest("Nome de usuário já está em uso");

                if (await _userManager.FindByEmailAsync(request.Mail) is not null)
                    return ResponseBadRequest("Email já está em uso");

                var user = new AppUser
                {
                    // Id int é identity (ValueGeneratedOnAdd). Não atribua manualmente.
                    UserName = request.UserName,
                    Email = request.Mail,
                    PhoneNumber = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password); // usa teu PBKDF2 via adapter
                if (!result.Succeeded)
                    return ResponseBadRequest(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Falha ao registrar o usuário");

                // Vincula com tua Account de domínio (1–1 por UserId:int)
                var account = new Account
                {
                    // Id int também deve ser identity na tabela (ValueGeneratedOnAdd) — não setar aqui.
                    UserId = user.Id, // FK 1–1 para AppUser.Id (int)
                    UserName = request.UserName,
                    Mail = request.Mail,
                    Phone = request.Phone,
                    AccountPermission = request.AccountPermission,
                    PasswordHash = _hash.HashPassword(request.Password), // manter compat. com sua coluna atual
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Account.Add(account);
                await _db.SaveChangesAsync();

                var resp = new AccountCreatedResponse
                {
                    UserName = request.UserName,
                    Mail = request.Mail,
                    Phone = request.Phone,
                    AccountPermission = request.AccountPermission
                };

                return ResponseCreated(resp, "Conta registrada com sucesso");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>Renova o token JWT</summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var validation = ValidateModelState();
                if (validation is not null) return validation;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // ignora expiração para extrair claims
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out _);

                var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr))
                    return ResponseUnauthorized("Token inválido");

                // Id é int
                if (!int.TryParse(userIdStr, out var userId))
                    return ResponseUnauthorized("Token inválido");

                var user = await _userManager.FindByIdAsync(userIdStr);
                if (user is null || !user.IsActive)
                    return ResponseUnauthorized("Usuário não encontrado");

                var account = await _db.Account.FirstOrDefaultAsync(a => a.UserId == userId);
                if (account is null) return ResponseUnauthorized("Conta não encontrada");

                var newToken = GenerateJwtToken(user, account);
                var refreshResponse = new RefreshTokenResponse
                {
                    Token = newToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(2)
                };

                return ResponseOk(refreshResponse, "Token renovado com sucesso");
            }
            catch (SecurityTokenException)
            {
                return ResponseUnauthorized("Token inválido ou expirado");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>Informações do usuário autenticado</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr))
                    return ResponseNotFound("Não autenticado");

                if (!int.TryParse(userIdStr, out var userId))
                    return ResponseNotFound("Token inválido");

                var account = await _db.Account.FirstOrDefaultAsync(a => a.UserId == userId);
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

                return ResponseOk(dto, "Ok");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>Altera a senha do usuário autenticado</summary>
        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var validation = ValidateModelState();
                if (validation is not null) return validation;

                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr)) return ResponseUnauthorized("Não autenticado");

                var user = await _userManager.FindByIdAsync(userIdStr);
                if (user is null || !user.IsActive) return ResponseUnauthorized("Usuário inválido");

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                    return ResponseBadRequest(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Não foi possível alterar a senha");

                // manter Account.PasswordHash em sincronia enquanto a coluna existir
                var account = await _db.Account.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (account is not null)
                {
                    account.PasswordHash = _hash.HashPassword(request.NewPassword);
                    account.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }

                return ResponseOk(account, "Senha alterada com sucesso");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>Gera o JWT com as claims esperadas pela SPA</summary>
        private string GenerateJwtToken(AppUser user, Account account)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
