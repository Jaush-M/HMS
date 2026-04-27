// Author: Salaams
using AutoMapper;
using HMS.Application.DTOs.Bookings;
using HMS.Application.DTOs.Users;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Services;

namespace HMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IBookingRepository _bookings;
    private readonly IMapper _mapper;

    public UserService(IUserRepository users, IBookingRepository bookings, IMapper mapper)
    {
        _users    = users;
        _bookings = bookings;
        _mapper   = mapper;
    }

    public async Task<GuestUserDto?> GetGuestByIdAsync(int id)
    {
        var guest = await _users.GetGuestByIdAsync(id);
        return guest is null ? null : _mapper.Map<GuestUserDto>(guest);
    }

    public async Task<GuestUserDto> UpdateGuestProfileAsync(int guestId, UpdateGuestProfileDto dto)
    {
        var guest = await _users.GetGuestByIdAsync(guestId)
            ?? throw new KeyNotFoundException($"Guest {guestId} not found.");

        guest.FirstName = dto.FirstName;
        guest.LastName  = dto.LastName;
        guest.Phone     = dto.Phone;
        guest.Address   = dto.Address;

        await _users.UpdateAsync(guest);
        return _mapper.Map<GuestUserDto>(guest);
    }

    public async Task<IEnumerable<BookingDto>> GetGuestBookingsAsync(int guestId)
    {
        var bookings = await _bookings.GetByGuestIdAsync(guestId);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<IEnumerable<StaffUserDto>> GetAllStaffAsync()
    {
        var staff = await _users.GetAllStaffAsync();
        return _mapper.Map<IEnumerable<StaffUserDto>>(staff);
    }

    public async Task<StaffUserDto?> GetStaffByIdAsync(int id)
    {
        var staff = await _users.GetStaffByIdAsync(id);
        return staff is null ? null : _mapper.Map<StaffUserDto>(staff);
    }
}
