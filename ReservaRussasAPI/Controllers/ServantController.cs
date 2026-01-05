using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.Entities;
using RR.Core.Services.Base;

namespace ReservaRussasAPI.Controllers
{
    public class ServantController : BaseControllerFYP
    {
        private readonly IServantService _servantService;

        public ServantController(IServantService servantService)
        {
            _servantService = servantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
              var servant = await _servantService.GetAllAsync();
              return ResponseOk(servant);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
        
        [HttpPost]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> AddAsync([FromBody] Servant servant)
        {
            try
            {
                if (servant == null)
                    return ResponseBadRequest("Servant data is required");

                var ok = await _servantService.AddAsync(servant);
                return ResponseOk(ok);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetServantById(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var servant = await _servantService.GetServantById(id);
                if (servant == null)
                    return ResponseNotFound("Servant not found");

                return ResponseOk(servant);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpGet("account/{id}")]
        [Authorize]
        public async Task<IActionResult> GetServantByAccountId(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var servant = await _servantService.GetServantByAccountId(id);
                if (servant == null)
                    return ResponseNotFound("Servant not found");

                return ResponseOk(servant);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Servant servant)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                if (servant == null)
                    return ResponseBadRequest("Servant data is required");

                servant.Id = id;

                var updated = await _servantService.UpdateAsync(servant);
                if (updated == null)
                    return ResponseNotFound("Servant not found");

                return ResponseOk(updated);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var deleted = await _servantService.DeleteAsync(id);
                if (!deleted)
                    return ResponseNotFound("Servant not found or already inactive");

                return ResponseOk(deleted);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
    }
}
