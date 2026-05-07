// Author: Salaams
using HMS.Application.DTOs.Hotels;
using HMS.Application.DTOs.Users;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

/// <summary>
/// Administrative endpoints restricted to the Admin role.
/// Phase 6 adds [Authorize(Roles = "Admin")] and user-management write operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IUserService _userService;

    public AdminController(IHotelService hotelService, IUserService userService)
    {
        _hotelService = hotelService;
        _userService  = userService;
    }

    /// <summary>Returns all active hotels (admin overview).</summary>
    [HttpGet("hotels")]
    [ProducesResponseType(typeof(IEnumerable<HotelSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HotelSummaryDto>>> GetHotels()
    {
        var hotels = await _hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    /// <summary>Returns all staff members across all hotels.</summary>
    [HttpGet("staff")]
    [ProducesResponseType(typeof(IEnumerable<StaffUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffUserDto>>> GetStaff()
    {
        var staff = await _userService.GetAllStaffAsync();
        return Ok(staff);
    }

    /// <summary>Returns a staff member by ID.</summary>
    [HttpGet("staff/{id:int}")]
    [ProducesResponseType(typeof(StaffUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffUserDto>> GetStaffById(int id)
    {
        var staff = await _userService.GetStaffByIdAsync(id);
        return staff is null ? NotFound($"Staff {id} not found.") : Ok(staff);
    }
}
