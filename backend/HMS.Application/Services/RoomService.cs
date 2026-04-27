// Author: Salaams
using AutoMapper;
using HMS.Application.DTOs.Rooms;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Services;

namespace HMS.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _rooms;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository rooms, IMapper mapper)
    {
        _rooms  = rooms;
        _mapper = mapper;
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var room = await _rooms.GetByIdAsync(id);
        return room is null ? null : _mapper.Map<RoomDto>(room);
    }

    public async Task<IEnumerable<RoomDto>> SearchAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut, int? minCapacity = null)
    {
        var rooms = await _rooms.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);
        if (minCapacity.HasValue)
            rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }
}
