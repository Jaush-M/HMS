// Author: Salaams
using HMS.Application.DTOs.Rooms;

namespace HMS.Application.Interfaces.Services;

public interface IRoomService
{
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<IEnumerable<RoomDto>> SearchAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut, int? minCapacity = null);
}
