// Author: Salaams
using HMS.Domain.Entities;

namespace HMS.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut);
    Task UpdateAsync(Room room);
}
