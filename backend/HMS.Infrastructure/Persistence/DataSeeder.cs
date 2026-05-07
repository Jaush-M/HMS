// Author: Salaams
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BcryptHelper = BCrypt.Net.BCrypt;

namespace HMS.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HmsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<HmsDbContext>>();

        await db.Database.MigrateAsync();

        // ── Phase 6: replace placeholder password hashes with real BCrypt hashes ─
        // Default test credentials:
        //   admin@grandplaza.com    → Admin@1234!
        //   manager@grandplaza.com  → Manager@1234!
        //   staff@grandplaza.com    → Staff@1234!
        //   guest@example.com       → Guest@1234!
        const string placeholder = "PLACEHOLDER_HASH_REPLACE_IN_PHASE_6";
        var withPlaceholder = await db.Users
            .Where(u => u.PasswordHash == placeholder)
            .ToListAsync();

        if (withPlaceholder.Any())
        {
            var passwordMap = new Dictionary<string, string>
            {
                ["admin@grandplaza.com"]   = "Admin@1234!",
                ["manager@grandplaza.com"] = "Manager@1234!",
                ["staff@grandplaza.com"]   = "Staff@1234!",
                ["guest@example.com"]      = "Guest@1234!",
            };
            foreach (var u in withPlaceholder)
            {
                var plain = passwordMap.TryGetValue(u.Email, out var p) ? p : "Default@1234!";
                u.PasswordHash = BcryptHelper.HashPassword(plain, workFactor: 12);
            }
            await db.SaveChangesAsync();
            logger.LogInformation("Replaced {Count} placeholder password hash(es).", withPlaceholder.Count);
        }

        if (await db.Hotels.AnyAsync()) return;

        logger.LogInformation("Seeding database...");

        // ── Hotels ────────────────────────────────────────────────────────────
        var hotels = new List<Hotel>
        {
            new() { Name = "Grand Plaza London",  City = "London",   Country = "UK",     Address = "1 Oxford Street, London W1D 1AN",          Phone = "+44 20 7946 0958", Email = "london@grandplaza.com" },
            new() { Name = "Grand Plaza Paris",   City = "Paris",    Country = "France", Address = "15 Avenue des Champs-Élysées, 75008 Paris", Phone = "+33 1 40 20 50 50", Email = "paris@grandplaza.com" },
            new() { Name = "Grand Plaza New York", City = "New York", Country = "USA",    Address = "350 Fifth Avenue, New York NY 10118",       Phone = "+1 212 736 3100",  Email = "nyc@grandplaza.com"   },
        };
        db.Hotels.AddRange(hotels);
        await db.SaveChangesAsync();

        // ── Rooms ─────────────────────────────────────────────────────────────
        var rooms = new List<Room>();
        foreach (var hotel in hotels)
        {
            rooms.AddRange(new[]
            {
                new Room { HotelId = hotel.Id, RoomNumber = "101", Type = RoomType.StandardDouble, Capacity = 2, PriceOffPeak = 120, PricePeak = 180, FloorNumber = 1, Status = RoomStatus.Available, Description = "Cosy standard double room with city view." },
                new Room { HotelId = hotel.Id, RoomNumber = "102", Type = RoomType.StandardDouble, Capacity = 2, PriceOffPeak = 120, PricePeak = 180, FloorNumber = 1, Status = RoomStatus.Available, Description = "Cosy standard double room with garden view." },
                new Room { HotelId = hotel.Id, RoomNumber = "201", Type = RoomType.DeluxeKing,     Capacity = 2, PriceOffPeak = 180, PricePeak = 250, FloorNumber = 2, Status = RoomStatus.Available, Description = "Spacious deluxe king room with premium amenities." },
                new Room { HotelId = hotel.Id, RoomNumber = "202", Type = RoomType.DeluxeKing,     Capacity = 2, PriceOffPeak = 180, PricePeak = 250, FloorNumber = 2, Status = RoomStatus.Cleaning,  Description = "Spacious deluxe king room with panoramic view." },
                new Room { HotelId = hotel.Id, RoomNumber = "301", Type = RoomType.FamilySuite,    Capacity = 4, PriceOffPeak = 240, PricePeak = 320, FloorNumber = 3, Status = RoomStatus.Available, Description = "Family suite with two bedrooms and a lounge area." },
                new Room { HotelId = hotel.Id, RoomNumber = "401", Type = RoomType.Penthouse,      Capacity = 4, PriceOffPeak = 500, PricePeak = 750, FloorNumber = 4, Status = RoomStatus.Available, Description = "Luxurious penthouse with private terrace and butler service." },
            });
        }
        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();

        // ── Users (one per role) ───────────────────────────────────────────────
        // Default credentials (change in production):
        //   admin@grandplaza.com   → Admin@1234!
        //   manager@grandplaza.com → Manager@1234!
        //   staff@grandplaza.com   → Staff@1234!
        //   guest@example.com      → Guest@1234!
        var admin = new StaffUser
        {
            Email = "admin@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Admin@1234!", workFactor: 12),
            Role = UserRole.Admin,
            FirstName = "Adam",
            LastName = "Admin",
            EmployeeId = "EMP001",
            Department = "IT",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        var manager = new StaffUser
        {
            Email = "manager@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Manager@1234!", workFactor: 12),
            Role = UserRole.HotelManager,
            FirstName = "Mary",
            LastName = "Manager",
            EmployeeId = "EMP002",
            Department = "Management",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        var staff = new StaffUser
        {
            Email = "staff@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Staff@1234!", workFactor: 12),
            Role = UserRole.FrontDeskStaff,
            FirstName = "Sam",
            LastName = "Staff",
            EmployeeId = "EMP003",
            Department = "Front Desk",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        var guest = new GuestUser
        {
            Email = "guest@example.com",
            PasswordHash = BcryptHelper.HashPassword("Guest@1234!", workFactor: 12),
            Role = UserRole.Guest,
            FirstName = "Grace",
            LastName = "Guest",
            Phone = "+44 7700 900123",
            Address = "42 Sample Street, London E1 6RF",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        db.Users.AddRange(admin, manager, staff, guest);
        await db.SaveChangesAsync();

        // ── Ancillary Services ────────────────────────────────────────────────
        var services = new List<AncillaryService>
        {
            new() { Name = "Airport Transfer",         Description = "One-way airport transfer by private car.",    Fee = 50,  Unit = "per trip"           },
            new() { Name = "Full English Breakfast",   Description = "Traditional full English breakfast.",          Fee = 20,  Unit = "per person per day" },
            new() { Name = "Spa Access",               Description = "Full day access to the hotel spa facilities.", Fee = 35,  Unit = "per person per day" },
            new() { Name = "Late Check-out (2 PM)",    Description = "Extend check-out until 2:00 PM.",             Fee = 40,  Unit = "per booking"        },
        };
        db.AncillaryServices.AddRange(services);
        await db.SaveChangesAsync();

        // ── Sample Booking ─────────────────────────────────────────────────────
        var londonHotel = hotels[0];
        var londonDeluxe = rooms.First(r => r.HotelId == londonHotel.Id && r.Type == RoomType.DeluxeKing && r.Status == RoomStatus.Available);

        var booking = new Booking
        {
            GuestId = guest.Id,
            HotelId = londonHotel.Id,
            CheckInDate = DateTime.UtcNow.Date.AddDays(30),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(33),
            Status = BookingStatus.Confirmed,
            TotalAmount = 180 * 3,
            CancellationFee = 0,
            Notes = "Guest requested high floor.",
            CreatedAt = DateTime.UtcNow,
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        db.BookingRooms.Add(new BookingRoom
        {
            BookingId = booking.Id,
            RoomId = londonDeluxe.Id,
            PriceAtBooking = londonDeluxe.PriceOffPeak,
        });

        db.BookingServices.Add(new BookingService
        {
            BookingId = booking.Id,
            ServiceId = services[0].Id, // Airport Transfer
            Quantity = 1,
            ServiceDate = booking.CheckInDate,
            TotalFee = services[0].Fee,
        });

        await db.SaveChangesAsync();

        logger.LogInformation("Database seeding complete.");
    }
}
