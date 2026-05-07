// Author: Salaams
using HMS.Application.DTOs.Auth;

namespace HMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto, string ipAddress);
    Task<LoginResponseDto> RegisterGuestAsync(RegisterGuestDto dto, string ipAddress);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto, string ipAddress);
}
