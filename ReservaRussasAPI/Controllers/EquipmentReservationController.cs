using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Services;

namespace ReservaRussasAPI.Controllers
{
    public class EquipmentReservationController : BaseControllerFYP
    {
        private readonly IEquipmentReservationService _equipmentReservations;

        public EquipmentReservationController(IEquipmentReservationService equipmentReservations)
        {
            _equipmentReservations = equipmentReservations;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _equipmentReservations.GetAllAsync();
            return ResponseOk(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _equipmentReservations.GetReservationById(id);
            if (res is null) return ResponseNotFound("Equipment reservation not found");
            return ResponseOk(res);
        }

        [HttpGet("equipment/{equipmentId:int}")]
        public async Task<IActionResult> GetByEquipment(int equipmentId)
        {
            var list = await _equipmentReservations.GetReservationsByEquipmentId(equipmentId);
            return ResponseOk(list);
        }

        [HttpGet("account/{accountId:int}")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            var list = await _equipmentReservations.GetReservationsByAccountId(accountId);
            return ResponseOk(list);
        }

        [HttpPost("period")]
        [AllowAnonymous]
        public async Task<IActionResult> ByPeriod([FromBody] PeriodRequest req)
        {
            var list = await _equipmentReservations.GetReservationsByPeriod(req.Start, req.End);
            return ResponseOk(list);
        }

        [HttpPost("availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability([FromBody] EquipmentAvailabilityRequest req)
        {
            var available = await _equipmentReservations.IsEquipmentAvailable(
                req.EquipmentId, req.Start, req.End);
            return ResponseOk(new { available });
        }

        [HttpPost]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentReservationRequest req)
        {
            var created = await _equipmentReservations.AddAsync(new EquipmentReservation
            {
                EquipmentId = req.EquipmentId,
                AccountId = req.AccountId,
                RoomReservationId = req.RoomReservationId,
                Title = req.Title,
                Description = req.Description,
                StartTime = req.StartTime,
                EndTime = req.EndTime
            });

            return ResponseOk(created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentReservationUpdateRequest req)
        {
            if (id != req.Id) return ResponseBadRequest("Route id and body id mismatch");
            
            var updated = await _equipmentReservations.UpdateAsync(new EquipmentReservation
            {
                Id = req.Id,
                EquipmentId = req.EquipmentId,
                AccountId = req.AccountId,
                StartTime = req.StartTime.DateTime,
                EndTime = req.EndTime.DateTime
            });

            if (updated is null) return ResponseNotFound("Equipment reservation not found");
            return ResponseOk(updated);
        }

        [HttpPut("cancel/{id:int}")]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> Cancel(int id)
        {
            var canceled = await _equipmentReservations.CancelReservation(id);
            if (!canceled) return ResponseNotFound("Equipment reservation not found or cannot be canceled");
            return ResponseOk(canceled);
        }

        [HttpPut("approve/{id:int}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> Approve(int id)
        {
            var approved = await _equipmentReservations.ApproveReservation(id);
            if (approved is null) return ResponseNotFound("Equipment reservation not found or cannot be approved");
            return ResponseOk(approved);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "ServantOrAbove")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _equipmentReservations.DeleteAsync(id);
            if (!ok) return ResponseBadRequest("Equipment reservation not found");
            return ResponseOk(true);
        }
    }
}
