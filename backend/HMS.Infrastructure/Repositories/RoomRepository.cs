// Author: Salaams
using HMS.Application.Interfaces.Repositories;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly HmsDbContext _db;
    public RoomRepository(HmsDbContext db) => _db = db;

    public async Task<Room?> GetByIdAsync(int id) =>
        await _db.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId) =>
        await _db.Rooms
            .Include(r => r.Hotel)
            .Where(r => r.HotelId == hotelId)
            .OrderBy(r => r.FloorNumber).ThenBy(r => r.RoomNumber)
            .ToListAsync();

    /// <summary>
    /// Returns rooms that are Available and have no overlapping confirmed/pending bookings.
    /// Uses a closed-open interval comparison: booking overlaps when
    ///   booking.CheckIn &lt; requestedCheckOut AND booking.CheckOut &gt; requestedCheckIn
    /// </summary>
    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut) =>
        await _db.Rooms
            .Include(r => r.Hotel)
            .Where(r => r.HotelId == hotelId && r.Status == RoomStatus.Available)
            .Where(r => !r.BookingRooms.Any(br =>
                br.Booking.Status != BookingStatus.Cancelled &&
                br.Booking.CheckInDate  < checkOut &&
                br.Booking.CheckOutDate > checkIn))
            .OrderBy(r => r.FloorNumber).ThenBy(r => r.RoomNumber)
            .ToListAsync();

    public async Task UpdateAsync(Room room)
    {
        _db.Rooms.Update(room);
        await _db.SaveChangesAsync();
    }
}
