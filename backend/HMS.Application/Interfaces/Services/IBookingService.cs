// Author: Salaams
using HMS.Application.DTOs.Bookings;

namespace HMS.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingDto>> GetBookingsByGuestAsync(int guestId);
    Task<IEnumerable<BookingDto>> GetBookingsByHotelAsync(int hotelId);
    Task<BookingDto> CreateBookingAsync(int guestId, CreateBookingDto dto);
    Task<BookingDto> CancelBookingAsync(int bookingId, int requestingUserId);
    Task<BookingDto> CheckInAsync(int bookingId, int staffId);
    Task<BookingDto> CheckOutAsync(int bookingId, int staffId);
}
