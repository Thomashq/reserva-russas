using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Services;

namespace ReservaRussasAPI.Controllers
{
    public class RoomController : BaseControllerFYP
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var rooms = await _roomService.GetAllAsync();
                return ResponseOk(rooms);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");
                var room = await _roomService.GetRoomById(id);
                if (room == null)
                    return ResponseNotFound("Room not found");
                return ResponseOk(room);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
        [HttpPost]
        [Authorize (Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> AddAsync([FromBody] CreateRoomRequest room)
        {
            try
            {
                if (room == null)
                    return ResponseBadRequest("Room data is required");

                Rooms entity = new Rooms
                {
                    Name = room.Name,
                    Capacity = room.Capacity,
                    ManagerId = room.ManagerId
                };

                var createdRoom = await _roomService.AddAsync(entity);
                return ResponseOk(createdRoom);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
        [HttpDelete("{id}")]
        [Authorize (Policy = "ManagerOrAdmin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");
                var deleted = await _roomService.DeleteAsync(id);
                if (!deleted)
                    return ResponseNotFound("Room not found or already inactive");
                return ResponseOk(deleted);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
        [HttpGet("period")]
        public async Task<IActionResult> GetRoomsReservationsByPeriod(int id, DateTime start, DateTime end)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");
                if (start >= end)
                    return ResponseBadRequest("Start time must be before end time");
                var room = await _roomService.GetRoomsReservationsByPeriod(id, start, end);
                if (room == null)
                    return ResponseNotFound("Room not found");
                return ResponseOk(room);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
    }
}
