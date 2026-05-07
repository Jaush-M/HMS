// Author: Salaams
using System.Security.Claims;
using HMS.Application.DTOs.Bookings;
using HMS.Application.DTOs.Invoices;
using HMS.Application.DTOs.Payments;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;

    public BookingsController(IBookingService bookingService, IPaymentService paymentService)
    {
        _bookingService = bookingService;
        _paymentService = paymentService;
    }

    /// <summary>Returns a booking by ID (with rooms, services, payments).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        return booking is null ? NotFound($"Booking {id} not found.") : Ok(booking);
    }

    /// <summary>Returns all bookings for a guest.</summary>
    [HttpGet("guest/{guestId:int}")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByGuest(int guestId)
    {
        var bookings = await _bookingService.GetBookingsByGuestAsync(guestId);
        return Ok(bookings);
    }

    /// <summary>Returns all bookings for a hotel (staff/manager view).</summary>
    [HttpGet("hotel/{hotelId:int}")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByHotel(int hotelId)
    {
        var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId);
        return Ok(bookings);
    }

    /// <summary>
    /// Creates a new booking. Guest ID is extracted from the JWT claims.
    /// Staff may also create bookings on behalf of guests.
    /// </summary>
    [HttpPost("guest/{guestId:int}")]
    [Authorize(Roles = "Guest,FrontDeskStaff,HotelManager,Admin")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> Create(int guestId, [FromBody] CreateBookingDto dto)
    {
        // Guests can only create bookings for themselves; staff can book on behalf of any guest
        var callerRole = User.FindFirst("role")?.Value ?? string.Empty;
        var callerId   = int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : 0;
        if (callerRole == "Guest" && callerId != guestId)
            return Forbid();

        try
        {
            var booking = await _bookingService.CreateBookingAsync(guestId, dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Cancels a booking. Cancellation fee rules applied in Phase 5.</summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> Cancel(int id)
    {
        try
        {
            // requestingUserId will come from JWT claims in Phase 6
            var booking = await _bookingService.CancelBookingAsync(id, requestingUserId: 0);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Returns all payments for a booking.</summary>
    [HttpGet("{id:int}/payments")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(int id)
    {
        var payments = await _paymentService.GetPaymentsByBookingAsync(id);
        return Ok(payments);
    }

    /// <summary>Returns the invoice for a booking.</summary>
    [HttpGet("{id:int}/invoice")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var invoice = await _paymentService.GetInvoiceByBookingAsync(id);
        return invoice is null ? NotFound($"No invoice found for booking {id}.") : Ok(invoice);
    }
}
