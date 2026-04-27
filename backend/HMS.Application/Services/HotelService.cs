// Author: Salaams
using AutoMapper;
using HMS.Application.DTOs.Hotels;
using HMS.Application.DTOs.Rooms;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Services;

namespace HMS.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotels;
    private readonly IRoomRepository _rooms;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotels, IRoomRepository rooms, IMapper mapper)
    {
        _hotels = hotels;
        _rooms  = rooms;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelSummaryDto>> GetAllHotelsAsync()
    {
        var hotels = await _hotels.GetAllAsync();
        return _mapper.Map<IEnumerable<HotelSummaryDto>>(hotels);
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await _hotels.GetByIdAsync(id);
        return hotel is null ? null : _mapper.Map<HotelDto>(hotel);
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsForHotelAsync(int hotelId)
    {
        var rooms = await _rooms.GetByHotelIdAsync(hotelId);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }
}
