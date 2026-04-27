// Author: Salaams
using HMS.Application.Interfaces.Repositories;
using HMS.Infrastructure.Persistence;
using HMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in configuration.");

        services.AddDbContext<HmsDbContext>(options =>
            options.UseNpgsql(connectionString));

        // ── Repository registrations ──────────────────────────────────────────
        services.AddScoped<IHotelRepository,            HotelRepository>();
        services.AddScoped<IRoomRepository,             RoomRepository>();
        services.AddScoped<IBookingRepository,          BookingRepository>();
        services.AddScoped<IUserRepository,             UserRepository>();
        services.AddScoped<IPaymentRepository,          PaymentRepository>();
        services.AddScoped<IInvoiceRepository,          InvoiceRepository>();
        services.AddScoped<IAncillaryServiceRepository, AncillaryServiceRepository>();

        return services;
    }
}