// Author: Salaams
using HMS.Application.DTOs.Bookings;
using HMS.Application.DTOs.Users;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    // ── Guests ────────────────────────────────────────────────────────────────

    /// <summary>Returns a guest's profile.</summary>
    [HttpGet("guests/{id:int}")]
    [ProducesResponseType(typeof(GuestUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestUserDto>> GetGuest(int id)
    {
        var guest = await _userService.GetGuestByIdAsync(id);
        return guest is null ? NotFound($"Guest {id} not found.") : Ok(guest);
    }

    /// <summary>Updates a guest's profile (name, phone, address).</summary>
    [HttpPut("guests/{id:int}")]
    [ProducesResponseType(typeof(GuestUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestUserDto>> UpdateGuest(
        int id, [FromBody] UpdateGuestProfileDto dto)
    {
        try
        {
            var guest = await _userService.UpdateGuestProfileAsync(id, dto);
            return Ok(guest);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    /// <summary>Returns all bookings for a guest.</summary>
    [HttpGet("guests/{id:int}/bookings")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetGuestBookings(int id)
    {
        var bookings = await _userService.GetGuestBookingsAsync(id);
        return Ok(bookings);
    }

    // ── Staff ─────────────────────────────────────────────────────────────────

    /// <summary>Returns all staff members.</summary>
    [HttpGet("staff")]
    [ProducesResponseType(typeof(IEnumerable<StaffUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffUserDto>>> GetAllStaff()
    {
        var staff = await _userService.GetAllStaffAsync();
        return Ok(staff);
    }

    /// <summary>Returns a staff member by ID.</summary>
    [HttpGet("staff/{id:int}")]
    [ProducesResponseType(typeof(StaffUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffUserDto>> GetStaff(int id)
    {
        var staff = await _userService.GetStaffByIdAsync(id);
        return staff is null ? NotFound($"Staff member {id} not found.") : Ok(staff);
    }
}
