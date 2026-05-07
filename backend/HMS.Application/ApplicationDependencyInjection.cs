// Author: Salaams
using AutoMapper;
using HMS.Application.Interfaces.Services;
using HMS.Application.Mappings;
using HMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;
// IAuthService registration added in Phase 6

namespace HMS.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper — singleton; profile is stateless so one instance is fine
        services.AddSingleton<IMapper>(new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>()).CreateMapper());

        // Application-layer services
        services.AddScoped<IAuthService,            AuthService>();
        services.AddScoped<IHotelService,           HotelService>();
        services.AddScoped<IRoomService,            RoomService>();
        services.AddScoped<IBookingService,         BookingManagementService>();
        services.AddScoped<IUserService,            UserService>();
        services.AddScoped<IPaymentService,         PaymentService>();
        services.AddScoped<IAncillaryServiceService, AncillaryServiceService>();
        services.AddScoped<IReportService,          ReportService>();

        return services;
    }
}
