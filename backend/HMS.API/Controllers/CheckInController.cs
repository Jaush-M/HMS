// Author: Salaams
using HMS.Application.DTOs.Bookings;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CheckInController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public CheckInController(IBookingService bookingService) => _bookingService = bookingService;

    /// <summary>
    /// Checks in a confirmed booking.
    /// NOTE: staffId will be extracted from JWT claims in Phase 6.
    /// </summary>
    [HttpPost("{bookingId:int}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> CheckIn(
        int bookingId, [FromQuery] int staffId = 0)
    {
        try
        {
            var booking = await _bookingService.CheckInAsync(bookingId, staffId);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Checks out a booking that is currently checked in.
    /// NOTE: staffId will be extracted from JWT claims in Phase 6.
    /// </summary>
    [HttpPost("{bookingId:int}/checkout")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> CheckOut(
        int bookingId, [FromQuery] int staffId = 0)
    {
        try
        {
            var booking = await _bookingService.CheckOutAsync(bookingId, staffId);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}
