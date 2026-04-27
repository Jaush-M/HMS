// Author: Salaams
using HMS.Application.DTOs.Rooms;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService) => _roomService = roomService;

    /// <summary>Returns a single room by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        return room is null ? NotFound($"Room {id} not found.") : Ok(room);
    }

    /// <summary>
    /// Searches for available rooms in a hotel for the given date range.
    /// Optional minCapacity filter.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> SearchAvailable(
        [FromQuery] int hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int? minCapacity = null)
    {
        if (checkOut <= checkIn)
            return BadRequest("Check-out date must be after check-in date.");

        var rooms = await _roomService.SearchAvailableRoomsAsync(hotelId, checkIn, checkOut, minCapacity);
        return Ok(rooms);
    }
}
