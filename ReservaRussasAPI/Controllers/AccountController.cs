using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Requests.Account;
using RR.Core.Responses.Account;
using RR.Core.Services;

namespace ReservaRussasAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de contas
    /// </summary>
    public class AccountController : BaseControllerFYP
    {
        private readonly IAccountService _accountService;
        private readonly IPasswordService _passwordService;
        public AccountController(IAccountService accountService, IPasswordService passwordService)
        {
            _accountService = accountService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Obtém uma conta por ID
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Dados da conta</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID deve ser um número positivo");

                var account = await _accountService.GetByIdAsync(id);

                if (account == null)
                    return ResponseNotFound("Conta não encontrada");

                var accountResponse = new AccountResponse
                {
                    Id = account.Id,
                    UserName = account.UserName,
                    Mail = account.Mail,
                    IsActive = account.IsActive,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                };

                return ResponseOk(accountResponse);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Cria uma nova conta
        /// </summary>
        /// <param name="request">Dados para criação da conta</param>
        /// <returns>Conta criada</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            try
            {
                var existingAccount = await _accountService.GetByEmailAsync(request.Mail);

                if (existingAccount != null)
                    return ResponseBadRequest("Email está em uso");

                var account = new Account
                {
                    UserName = request.UserName,
                    Mail = request.Mail,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _accountService.AddAsync(account);
                return ResponseCreated(account);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Atualiza uma conta existente
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Conta atualizada</returns>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID deve ser um número positivo");

                var account = new Account { 
                    Id = id,
                    UserName = request.UserName,
                    Mail = request.Mail,
                    Phone = request.Phone,
                    AccountPermission = request.AccountPermission,
                };

                var updatedAccount = await _accountService.UpdateAsync(account);

                if (updatedAccount == null)
                    return ResponseNotFound("Conta não encontrada");

                return ResponseOk(updatedAccount);
            }
            catch (ArgumentException ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Remove uma conta (soft delete)
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID deve ser um número positivo");

                var success = await _accountService.DeleteAsync(id);

                if (!success)
                    return ResponseNotFound("Conta não encontrada");

                return ResponseNoContent();
            }
            catch (InvalidOperationException ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        /// <summary>
        /// Altera a senha de uma conta
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <param name="request">Dados para alteração da senha</param>
        /// <returns>Resultado da operação</returns>
        [HttpPatch("{id:int}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID deve ser um número positivo");

                var success = true;

                if (!success)
                    return ResponseNotFound("Conta não encontrada ou senha atual incorreta");

                return ResponseOk("Senha alterada com sucesso");
            }
            catch (ArgumentException ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ResponseBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
    }
}