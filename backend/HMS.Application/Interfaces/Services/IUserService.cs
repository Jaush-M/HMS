// Author: Salaams
using HMS.Application.DTOs.Bookings;
using HMS.Application.DTOs.Users;

namespace HMS.Application.Interfaces.Services;

public interface IUserService
{
    Task<GuestUserDto?> GetGuestByIdAsync(int id);
    Task<GuestUserDto> UpdateGuestProfileAsync(int guestId, UpdateGuestProfileDto dto);
    Task<IEnumerable<BookingDto>> GetGuestBookingsAsync(int guestId);
    Task<IEnumerable<StaffUserDto>> GetAllStaffAsync();
    Task<StaffUserDto?> GetStaffByIdAsync(int id);
}
