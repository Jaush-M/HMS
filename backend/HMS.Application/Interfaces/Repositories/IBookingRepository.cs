// Author: Salaams
using HMS.Domain.Entities;

namespace HMS.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Booking>> GetByGuestIdAsync(int guestId);
    Task<IEnumerable<Booking>> GetByHotelIdAsync(int hotelId);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}
