using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RR.Core.Services;
using RR.Core.Requests.Account;
using RR.Core.Responses.Account;
using RR.Core.Entities;
using ReservaRussasAPI.Controllers.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RR.Core.DTOs.Requests;
using RR.Core.DTOs.Responses;
using Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace ReservaRussasAPI.Controllers
{
    public class AuthController : BaseControllerFYP
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;
        private readonly IAccountService _accountService;

        public AuthController(IAuthService authService, IConfiguration configuration, IPasswordService passwordService, IAccountService accountService)
        {
            _authService = authService;
            _configuration = configuration;
            _passwordService = passwordService;
            _accountService = accountService;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="request">Dados de login</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Valida o modelo
                var validationResult = ValidateModelState();
                if (validationResult != null)
                {
                    return validationResult;
                }

                var account = await _authService.Login(request.UserName, request.Password);
                if (account == null)
                {
                    return ResponseUnauthorized("Usuário ou senha inválidos");
                }

                var token = GenerateJwtToken(account);
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

        /// <summary>
        /// Registra uma nova conta
        /// </summary>
        /// <param name="request">Dados para registro</param>
        /// <returns>Conta criada</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
        {
            try
            {
                // Valida o modelo
                var validationResult = ValidateModelState();
                if (validationResult != null)
                {
                    return validationResult;
                }

                // Verifica se o usuário já existe
                var existingAccount = await _accountService.GetByUsernameAsync(request.UserName);
                if (existingAccount != null)
                {
                    return ResponseBadRequest("Nome de usuário já está em uso");
                }

                // Verifica se o email já existe
                var existingEmail = await _accountService.GetByEmailAsync(request.Mail);
                if (existingEmail != null)
                {
                    return ResponseBadRequest("Email já está em uso");
                }

                var accountDto = new CreateAccountRequest
                {
                    UserName = request.UserName,
                    Mail = request.Mail,
                    Password = request.Password,
                    Phone = request.Phone,
                    AccountPermission = request.AccountPermission
                };

                var createdAccount = await _authService.Register(accountDto);
                if (createdAccount == null)
                {
                    return ResponseBadRequest("Não foi possível registrar a conta");
                }

                var accountCreatedResponse = new AccountCreatedResponse
                {
                    UserName = request.UserName,
                    Mail = request.Mail,
                    Phone = request.Phone,
                    AccountPermission = request.AccountPermission
                };

                return ResponseCreated(accountCreatedResponse, "Conta registrada com sucesso");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Renova o token JWT
        /// </summary>
        /// <param name="request">Token para renovação</param>
        /// <returns>Novo token JWT</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Valida o modelo
                var validationResult = ValidateModelState();
                if (validationResult != null)
                {
                    return validationResult;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                // Valida o token sem verificar expiração
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Não valida expiração para refresh
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out SecurityToken validatedToken);

                // Extrai as informações do token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = principal.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                {
                    return ResponseUnauthorized("Token inválido");
                }

                // Busca a conta no banco para verificar se ainda existe
                var account = await _accountService.GetByEmailAsync(userName);
                if (account == null)
                {
                    return ResponseUnauthorized("Usuário não encontrado");
                }

                // Gera um novo token
                var newToken = GenerateJwtToken(account);
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

        /// <summary>
        /// Obtém informações do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Implementar quando tiver autenticação por token
                // var userId = GetCurrentUserId(); // Método para extrair do token
                // var account = await _authService.GetByIdAsync(userId);

                // Por enquanto, retorna não implementado
                return ResponseNotFound("Funcionalidade não implementada");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Altera a senha do usuário
        /// </summary>
        /// <param name="request">Dados para alteração de senha</param>
        /// <returns>Resultado da operação</returns>
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Valida o modelo
                var validationResult = ValidateModelState();
                if (validationResult != null)
                {
                    return validationResult;
                }

                // Implementar quando tiver autenticação por token
                // var userId = GetCurrentUserId();
                // var success = await _authService.ChangePassword(userId, request.CurrentPassword, request.NewPassword);

                // Por enquanto, retorna não implementado
                return ResponseNotFound("Funcionalidade não implementada");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Gera token JWT para a conta
        /// </summary>
        /// <param name="account">Conta do usuário</param>
        /// <returns>Token JWT</returns>
        private string GenerateJwtToken(Account account)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Mail)
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