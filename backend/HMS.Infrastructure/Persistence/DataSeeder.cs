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

        if (await db.Hotels.AnyAsync()) return;

        logger.LogInformation("Seeding database...");

        // ── Resorts / Hotels ─────────────────────────────────────────────────
        var hotels = new List<Hotel>
        {
            new()
            {
                Name = "Grand Plaza Sunset Reef Hotel",
                City = "Maafushi",
                Country = "Maldives",
                Address = "Kaafu Atoll, Maldives",
                Phone = "+960 400 1002",
                Email = "sunsetreef@grandplaza.mv"
            },
            new()
            {
                Name = "Grand Plaza Coconut Breeze Retreat",
                City = "Addu City",
                Country = "Maldives",
                Address = "Addu Atoll, Maldives",
                Phone = "+960 400 1003",
                Email = "coconutbreeze@grandplaza.mv"
            },
            new()
            {
                Name = "Grand Plaza Pearl Sands Water Hotel",
                City = "Dhigurah",
                Country = "Maldives",
                Address = "South Ari Atoll, Maldives",
                Phone = "+960 400 1004",
                Email = "pearlsands@grandplaza.mv"
            }
        };

        var resorts = new List<Hotel>
        {
            new()
            {
                Name = "Grand Plaza Azure Lagoon Resort",
                City = "Male",
                Country = "Maldives",
                Address = "North Male Atoll, Maldives",
                Phone = "+960 400 1001",
                Email = "azurelagoon@grandplaza.mv"
            },
        };

        db.Hotels.AddRange(hotels);
        db.Hotels.AddRange(resorts);
        await db.SaveChangesAsync();

        // ── Rooms ─────────────────────────────────────────────────────────────
        var rooms = new List<Room>();

        // ──  Hotels ───────────────────────────────────────────────────────────
        foreach (var hotel in hotels)
        {
            rooms.AddRange(new[]
            {
                new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = "101",
                    Type = RoomType.StandardDouble,
                    Capacity = 2,
                    PriceOffPeak = 140,
                    PricePeak = 200,
                    FloorNumber = 1,
                    Status = RoomStatus.Available,
                    Description = "Comfortable standard room with island-inspired interior design."
                },

                new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = "102",
                    Type = RoomType.StandardDouble,
                    Capacity = 2,
                    PriceOffPeak = 160,
                    PricePeak = 220,
                    FloorNumber = 1,
                    Status = RoomStatus.Available,
                    Description = "Ocean-view standard room with balcony seating area."
                },

                new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = "201",
                    Type = RoomType.DeluxeKing,
                    Capacity = 2,
                    PriceOffPeak = 280,
                    PricePeak = 380,
                    FloorNumber = 2,
                    Status = RoomStatus.Available,
                    Description = "Spacious deluxe room with premium amenities and sea views."
                },

                new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = "202",
                    Type = RoomType.DeluxeKing,
                    Capacity = 2,
                    PriceOffPeak = 320,
                    PricePeak = 420,
                    FloorNumber = 2,
                    Status = RoomStatus.Cleaning,
                    Description = "Deluxe sunset-facing room with lounge area and minibar."
                },

                new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = "301",
                    Type = RoomType.FamilySuite,
                    Capacity = 4,
                    PriceOffPeak = 500,
                    PricePeak = 650,
                    FloorNumber = 3,
                    Status = RoomStatus.Available,
                    Description = "Family suite with two bedrooms, living space, and partial ocean view."
                }
            });
        }

        // ── Luxury Resorts ───────────────────────────────────────────────────
        foreach (var resort in resorts)
        {
            rooms.AddRange(new[]
            {
                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "101",
                    Type = RoomType.BeachVilla,
                    Capacity = 2,
                    PriceOffPeak = 650,
                    PricePeak = 900,
                    FloorNumber = 1,
                    Status = RoomStatus.Available,
                    Description = "Private beachfront villa with outdoor deck and tropical garden."
                },

                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "102",
                    Type = RoomType.BeachVilla,
                    Capacity = 2,
                    PriceOffPeak = 720,
                    PricePeak = 980,
                    FloorNumber = 1,
                    Status = RoomStatus.Available,
                    Description = "Luxury beach villa with plunge pool and direct beach access."
                },

                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "201",
                    Type = RoomType.WaterVilla,
                    Capacity = 2,
                    PriceOffPeak = 950,
                    PricePeak = 1350,
                    FloorNumber = 2,
                    Status = RoomStatus.Available,
                    Description = "Water villa built over the lagoon with glass floor panels."
                },

                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "301",
                    Type = RoomType.OverwaterBungalow,
                    Capacity = 2,
                    PriceOffPeak = 1200,
                    PricePeak = 1700,
                    FloorNumber = 3,
                    Status = RoomStatus.Available,
                    Description = "Luxury overwater bungalow with infinity pool and sunset views."
                },

                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "401",
                    Type = RoomType.HoneymoonVilla,
                    Capacity = 2,
                    PriceOffPeak = 1600,
                    PricePeak = 2200,
                    FloorNumber = 4,
                    Status = RoomStatus.Available,
                    Description = "Exclusive honeymoon villa with private butler service and floating breakfast."
                },

                new Room
                {
                    HotelId = resort.Id,
                    RoomNumber = "501",
                    Type = RoomType.PresidentialVilla,
                    Capacity = 6,
                    PriceOffPeak = 3500,
                    PricePeak = 5000,
                    FloorNumber = 5,
                    Status = RoomStatus.Available,
                    Description = "Grand presidential villa featuring private pool, ocean deck, spa room, and personal chef service."
                }
            });
        }

        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();

        // ── Users ────────────────────────────────────────────────────────────
        var admin = new StaffUser
        {
            Email = "admin@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Admin@1234!", workFactor: 12),
            Role = UserRole.Admin,
            FirstName = "Admin",
            LastName = "User",
            EmployeeId = "EMP001",
            Department = "Administration",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        var manager = new StaffUser
        {
            Email = "manager@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Manager@1234!", workFactor: 12),
            Role = UserRole.HotelManager,
            FirstName = "Aishath",
            LastName = "Latheef",
            EmployeeId = "EMP002",
            Department = "Resort Operations",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        var staff = new StaffUser
        {
            Email = "staff@grandplaza.com",
            PasswordHash = BcryptHelper.HashPassword("Staff@1234!", workFactor: 12),
            Role = UserRole.FrontDeskStaff,
            FirstName = "Mohamed",
            LastName = "Shifan",
            EmployeeId = "EMP003",
            Department = "Front Office",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        var guest = new GuestUser
        {
            Email = "guest@example.com",
            PasswordHash = BcryptHelper.HashPassword("Guest@1234!", workFactor: 12),
            Role = UserRole.Guest,
            FirstName = "Grace",
            LastName = "Taylor",
            Phone = "+960 7778899",
            Address = "M. Blue Reef, Male, Maldives",
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        db.Users.AddRange(admin, manager, staff, guest);
        await db.SaveChangesAsync();

        // ── Resort Services ──────────────────────────────────────────────────
        var services = new List<AncillaryService>
        {
            new()
            {
                Name = "Speedboat Transfer",
                Description = "Round-trip speedboat transfer from Velana International Airport.",
                Fee = 95,
                Unit = "per person"
            },

            new()
            {
                Name = "Seaplane Transfer",
                Description = "Luxury seaplane transfer to the resort island.",
                Fee = 350,
                Unit = "per person"
            },

            new()
            {
                Name = "Sunset Dolphin Cruise",
                Description = "Evening cruise with dolphin watching experience.",
                Fee = 80,
                Unit = "per person"
            },

            new()
            {
                Name = "Spa & Wellness Package",
                Description = "Relaxing spa treatment with sauna and massage.",
                Fee = 150,
                Unit = "per session"
            },

            new()
            {
                Name = "Scuba Diving Excursion",
                Description = "Guided scuba diving session at coral reef sites.",
                Fee = 200,
                Unit = "per trip"
            },

            new()
            {
                Name = "Floating Breakfast",
                Description = "Private floating breakfast served in villa pool.",
                Fee = 65,
                Unit = "per booking"
            }
        };

        db.AncillaryServices.AddRange(services);
        await db.SaveChangesAsync();

        // ── Sample Booking ───────────────────────────────────────────────────
        var selectedHotel = hotels[0];

        var selectedRoom = rooms.First(r =>
            r.HotelId == selectedHotel.Id &&
            r.Type == RoomType.DeluxeKing &&
            r.Status == RoomStatus.Available);

        var booking = new Booking
        {
            GuestId = guest.Id,
            HotelId = selectedHotel.Id,
            CheckInDate = DateTime.UtcNow.Date.AddDays(14),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(18),
            Status = BookingStatus.Confirmed,
            TotalAmount = 350 * 4,
            CancellationFee = 0,
            Notes = "Guest requested honeymoon villa decoration.",
            CreatedAt = DateTime.UtcNow,
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        db.BookingRooms.Add(new BookingRoom
        {
            BookingId = booking.Id,
            RoomId = selectedRoom.Id,
            PriceAtBooking = selectedRoom.PriceOffPeak,
        });

        db.BookingServices.AddRange(
            new BookingService
            {
                BookingId = booking.Id,
                ServiceId = services[0].Id,
                Quantity = 2,
                ServiceDate = booking.CheckInDate,
                TotalFee = services[0].Fee * 2,
            },

            new BookingService
            {
                BookingId = booking.Id,
                ServiceId = services[2].Id,
                Quantity = 2,
                ServiceDate = booking.CheckInDate.AddDays(1),
                TotalFee = services[2].Fee * 2,
            }
        );

        await db.SaveChangesAsync();

        logger.LogInformation("Database seeding complete.");
    }
}
