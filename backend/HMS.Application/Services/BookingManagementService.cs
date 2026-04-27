// Author: Salaams
using AutoMapper;
using HMS.Application.DTOs.Bookings;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Services;
using HMS.Domain.Entities;
using HMS.Domain.Enums;

namespace HMS.Application.Services;

/// <summary>
/// Handles booking lifecycle: create, cancel, check-in, check-out.
/// Business rule enforcement (availability, cancellation fees, pricing) is
/// expanded in Phase 5.
/// </summary>
public class BookingManagementService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IRoomRepository _rooms;
    private readonly IAncillaryServiceRepository _services;
    private readonly IMapper _mapper;

    public BookingManagementService(
        IBookingRepository bookings,
        IRoomRepository rooms,
        IAncillaryServiceRepository services,
        IMapper mapper)
    {
        _bookings = bookings;
        _rooms    = rooms;
        _services = services;
        _mapper   = mapper;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(id);
        return booking is null ? null : _mapper.Map<BookingDto>(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByGuestAsync(int guestId)
    {
        var bookings = await _bookings.GetByGuestIdAsync(guestId);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByHotelAsync(int hotelId)
    {
        var bookings = await _bookings.GetByHotelIdAsync(hotelId);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    /// <summary>
    /// Creates a pending booking for the given guest.
    /// Phase 5 adds: availability validation, peak/off-peak pricing, fee pre-calculation.
    /// </summary>
    public async Task<BookingDto> CreateBookingAsync(int guestId, CreateBookingDto dto)
    {
        var booking = new Booking
        {
            GuestId      = guestId,
            HotelId      = dto.HotelId,
            CheckInDate  = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Status       = BookingStatus.Pending,
            Notes        = dto.Notes,
            CreatedAt    = DateTime.UtcNow,
        };

        foreach (var roomId in dto.RoomIds)
        {
            var room = await _rooms.GetByIdAsync(roomId)
                ?? throw new KeyNotFoundException($"Room {roomId} not found.");

            // Phase 5: use peak/off-peak price based on check-in date
            booking.BookingRooms.Add(new BookingRoom
            {
                RoomId         = roomId,
                PriceAtBooking = room.PriceOffPeak,
            });
        }

        foreach (var svc in dto.Services)
        {
            // Phase 5: look up AncillaryService.Fee and multiply by Quantity
            booking.BookingServices.Add(new()
            {
                ServiceId   = svc.ServiceId,
                Quantity    = svc.Quantity,
                ServiceDate = svc.ServiceDate,
                TotalFee    = 0,
            });
        }

        await _bookings.AddAsync(booking);

        var created = await _bookings.GetByIdWithDetailsAsync(booking.Id)
            ?? throw new InvalidOperationException("Booking not found after creation.");
        return _mapper.Map<BookingDto>(created);
    }

    /// <summary>
    /// Cancels a booking. Cancellation fee rules are applied in Phase 5.
    /// </summary>
    public async Task<BookingDto> CancelBookingAsync(int bookingId, int requestingUserId)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(bookingId)
            ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

        if (booking.Status is BookingStatus.CheckedIn or BookingStatus.CheckedOut)
            throw new InvalidOperationException(
                "Cannot cancel a booking that has already checked in or checked out.");

        booking.Status = BookingStatus.Cancelled;
        // Phase 5: calculate fee based on notice period (14-day / 72-hour / no-show rules)
        booking.CancellationFee = 0;

        await _bookings.UpdateAsync(booking);
        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> CheckInAsync(int bookingId, int staffId)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(bookingId)
            ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be checked in.");

        booking.Status = BookingStatus.CheckedIn;
        await _bookings.UpdateAsync(booking);
        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> CheckOutAsync(int bookingId, int staffId)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(bookingId)
            ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

        if (booking.Status != BookingStatus.CheckedIn)
            throw new InvalidOperationException("Only checked-in bookings can be checked out.");

        booking.Status = BookingStatus.CheckedOut;
        // Phase 5: generate invoice, finalise TotalAmount
        await _bookings.UpdateAsync(booking);
        return _mapper.Map<BookingDto>(booking);
    }
}
