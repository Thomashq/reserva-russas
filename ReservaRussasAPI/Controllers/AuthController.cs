using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Services;
using System.Globalization;
using System.Security.Claims;

namespace ReservaRussasAPI.Controllers
{
    public class AuthController : BaseControllerFYP
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(
            IAuthService authService,
            IAccountService accountService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _authService = authService;
            _accountService = accountService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var account = await _authService.Login(request.UserName, request.Password);
            if (account is null) return ResponseUnauthorized("Usuário ou senha inválidos");

            var user = await _userManager.FindByIdAsync(account.UserId.ToString());
            if (user is null) return ResponseUnauthorized("Usuário não encontrado");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("accountId", account.Id.ToString(CultureInfo.InvariantCulture))
            };

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);
            return ResponseOk("Login realizado com sucesso");
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest request)
        {
          try{
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
          catch(Exception ex){
            throw new Exception("Não foi possível registrar conta", ex);
          }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var accountIdStr = User.FindFirst("accountId")?.Value;
            if (!int.TryParse(accountIdStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var accountId))
                return ResponseUnauthorized("Usuário não autenticado");

            var currentAccount = await _accountService.GetByIdAsync(accountId);
            if (currentAccount is null)
                return ResponseUnauthorized("Usuário não encontrado");

            return ResponseOk(currentAccount);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return ResponseOk("Logout realizado com sucesso");
        }

        [HttpGet("check")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAuthStatus()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var accountIdStr = User.FindFirst("accountId")?.Value;
                if (int.TryParse(accountIdStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var accountId))
                {
                    var account = await _accountService.GetByIdAsync(accountId);
                    return ResponseOk(new { isAuthenticated = true, account });
                }
            }

            return ResponseOk(new { isAuthenticated = false, account = (object?)null });
        }
    }
}
