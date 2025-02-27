using Core.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using RR.Core.DTOs;

namespace ReservaRussasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AccountDTO>>>> GetAll()
        {
            var result = await _accountService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<AccountDTO>>(result, true, "Accounts retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> GetById(Guid id)
        {
            var result = await _accountService.GetByIdAsync(id);
            return result != null ? Ok(new ApiResponse<AccountDTO>(result, true, "Account found"))
                                  : NotFound(new ApiResponse<AccountDTO>(null, false, "Account not found"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> Create([FromBody] AccountDTO dto)
        {
            var result = await _accountService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<AccountDTO>(result, true, "Account created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AccountDTO>>> Update(Guid id, [FromBody] AccountDTO dto)
        {
            var result = await _accountService.UpdateAsync(id, dto);
            return result != null ? Ok(new ApiResponse<AccountDTO>(result, true, "Account updated successfully"))
                                  : NotFound(new ApiResponse<AccountDTO>(null, false, "Account not found"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var success = await _accountService.DeleteAsync(id);
            return success ? Ok(new ApiResponse<bool>(true, true, "Account deleted successfully"))
                           : NotFound(new ApiResponse<bool>(false, false, "Account not found"));
        }
    }
}
