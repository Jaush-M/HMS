// Author: Salaams
using HMS.Application.DTOs.Reports;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Services;
using HMS.Domain.Enums;

namespace HMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IHotelRepository _hotels;
    private readonly IRoomRepository _rooms;
    private readonly IBookingRepository _bookings;

    public ReportService(
        IHotelRepository hotels,
        IRoomRepository rooms,
        IBookingRepository bookings)
    {
        _hotels   = hotels;
        _rooms    = rooms;
        _bookings = bookings;
    }

    public async Task<OccupancyReportDto> GetOccupancyReportAsync(
        int hotelId, DateTime from, DateTime to)
    {
        var hotel    = await _hotels.GetByIdAsync(hotelId);
        var rooms    = (await _rooms.GetByHotelIdAsync(hotelId)).ToList();
        var bookings = (await _bookings.GetByHotelIdAsync(hotelId)).ToList();

        // Bookings that overlap with [from, to] and are not cancelled
        var active = bookings
            .Where(b => b.Status != BookingStatus.Cancelled
                     && b.CheckInDate  < to
                     && b.CheckOutDate > from)
            .ToList();

        // Distinct rooms occupied across those bookings
        var occupiedRoomCount = active
            .SelectMany(b => b.BookingRooms.Select(br => br.RoomId))
            .Distinct()
            .Count();

        var total = rooms.Count;
        return new OccupancyReportDto
        {
            HotelId       = hotelId,
            HotelName     = hotel?.Name ?? string.Empty,
            From          = from,
            To            = to,
            TotalRooms    = total,
            OccupiedRooms = occupiedRoomCount,
            OccupancyRate = total == 0 ? 0 : Math.Round((double)occupiedRoomCount / total * 100, 1),
        };
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(
        int hotelId, DateTime from, DateTime to)
    {
        var hotel    = await _hotels.GetByIdAsync(hotelId);
        var bookings = (await _bookings.GetByHotelIdAsync(hotelId)).ToList();

        // Bookings within the date window with revenue-bearing statuses
        var qualifying = bookings
            .Where(b => b.Status is BookingStatus.CheckedOut
                                 or BookingStatus.CheckedIn
                                 or BookingStatus.Confirmed
                     && b.CheckInDate  >= from
                     && b.CheckInDate  <  to)
            .ToList();

        var total   = qualifying.Sum(b => b.TotalAmount);
        var count   = qualifying.Count;
        return new RevenueReportDto
        {
            HotelId             = hotelId,
            HotelName           = hotel?.Name ?? string.Empty,
            From                = from,
            To                  = to,
            TotalRevenue        = total,
            TotalBookings       = count,
            AverageBookingValue = count == 0 ? 0 : Math.Round(total / count, 2),
        };
    }
}
