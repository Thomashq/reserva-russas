using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.Common;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Services;

public class ReservationController : BaseControllerFYP
{
    private readonly IReservationService _reservations;

    public ReservationController(IReservationService reservations) => _reservations = reservations;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _reservations.GetAllAsync();
        return ResponseOk(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await _reservations.GetReservationById(id);
        if (res is null) return ResponseNotFound("Reservation not found");
        return ResponseOk(res);
    }

    [HttpGet("room/{roomId:int}")]
    public async Task<IActionResult> GetByRoom(int roomId)
    {
        var list = await _reservations.GetReservationsByRoomId(roomId);
        return ResponseOk(list);
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetByAccount(int accountId)
    {
        var list = await _reservations.GetReservationsByAccountId(accountId);
        return ResponseOk(list);
    }

    [HttpPost("period")]
    [AllowAnonymous]
    public async Task<IActionResult> ByPeriod([FromBody] PeriodRequest req)
    {
        var list = await _reservations.GetReservationsByPeriod(req.Start, req.End);
        return ResponseOk(list);
    }

    [HttpPost]
    [Authorize(Policy = "ServantOrAbove")]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest req)
    {
        var created = await _reservations.AddAsync(new Reservation
        {
            RoomId = req.RoomId,
            AccountId = req.AccountId,
            Title = req.Title,
            Description = req.Description,
            StartTime = req.StartTime,
            EndTime = req.EndTime
        });
            
        return ResponseOk(created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ServantOrAbove")]
    public async Task<IActionResult> Update(int id, [FromBody] ReservationUpdateRequest req)
    {
        if (id != req.Id) return ResponseBadRequest("Route id and body id mismatch");

        var updated = await _reservations.UpdateAsync(new Reservation
        {
            Id = req.Id,
            RoomId = req.RoomId,
            AccountId = req.AccountId,
            StartTime = req.StartTime.DateTime, 
            EndTime = req.EndTime.DateTime      
        });

        if (updated is null) return ResponseNotFound("Reservation not found");
        return ResponseOk(updated);
    }

    [HttpPut("cancel/{id:int}")]
    [Authorize(Policy = "ServantOrAbove")]
    public async Task<IActionResult> Cancel(int id)
    {
        var canceled = await _reservations.CancelReservation(id);
        if (canceled is false) return ResponseNotFound("Reservation not found or cannot be canceled");
        return ResponseOk(canceled);
    }

    [HttpPut("approve/{id:int}")]
    [Authorize(Policy ="ManagerOrAdmin")]
    public async Task<IActionResult> Approve(int id)
    {
        var approved = await _reservations.ApproveReservation(id);
        if (approved is null) return ResponseNotFound("Reservation not found or cannot be approved");
        return ResponseOk(approved);
    }

    [HttpDelete("{id:int}")]
    [Authorize (Policy = "ServantOrAbove")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _reservations.DeleteAsync(id);
        if (!ok) return ResponseBadRequest("Reservation not found");
        return ResponseOk(true);
    }
}
