using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Services;

namespace ReservaRussasAPI.Controllers
{
    public class EquipmentController : BaseControllerFYP
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _equipmentService.GetAllAsync();
            return ResponseOk(list);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var equipment = await _equipmentService.GetByIdAsync(id);
            if (equipment is null) return ResponseNotFound("Equipment not found");
            return ResponseOk(equipment);
        }

        [HttpGet("room/{roomId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRoom(int roomId)
        {
            var list = await _equipmentService.GetEquipmentsByRoomId(roomId);
            return ResponseOk(list);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentRequest req)
        {
            var equipment = new Equipment
            {
                Name = req.Name,
                Description = req.Description
            };

            var created = await _equipmentService.AddAsync(equipment);
            return ResponseOk(created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentRequest req)
        {
            if (id != req.Id) return ResponseBadRequest("Route id and body id mismatch");

            var equipment = new Equipment
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description
            };

            var updated = await _equipmentService.UpdateAsync(equipment);
            if (updated is null) return ResponseNotFound("Equipment not found");

            return ResponseOk(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _equipmentService.DeleteAsync(id);
            if (!ok) return ResponseBadRequest("Equipment not found or cannot be deleted");

            return ResponseOk(true);
        }
    }
}
