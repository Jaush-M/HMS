// Author: Salaams
using HMS.Application.BusinessRules;
using HMS.Application.DTOs.Auth;
using HMS.Application.Interfaces.Repositories;
using HMS.Application.Interfaces.Security;
using HMS.Application.Interfaces.Services;
using HMS.Domain.Entities;
using HMS.Domain.Enums;

namespace HMS.Application.Services;

/// <summary>
/// Handles authentication and account management:
///   Login   — credential validation, account-lockout policy, password-expiry check, JWT issuance
///   Register — guest self-registration with BCrypt hash and password-policy enforcement
///   ChangePassword — validates current password, enforces policy, resets expiry clock
/// </summary>
public class AuthService : IAuthService
{
    private const int  MaxFailedAttempts    = 5;
    private const int  LockoutMinutes       = 15;
    private const int  PasswordExpiryDays   = 180; // 6 months — enforced for Admin + HotelManager

    private readonly IUserRepository     _users;
    private readonly IAuditLogRepository _auditLogs;
    private readonly IPasswordHasher     _hasher;
    private readonly IJwtTokenService    _jwt;

    public AuthService(
        IUserRepository     users,
        IAuditLogRepository auditLogs,
        IPasswordHasher     hasher,
        IJwtTokenService    jwt)
    {
        _users     = users;
        _auditLogs = auditLogs;
        _hasher    = hasher;
        _jwt       = jwt;
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, string ipAddress)
    {
        var user = await _users.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        // Auto-unlock after lockout window expires
        if (user.IsLocked && user.LockedUntil.HasValue && user.LockedUntil <= DateTime.UtcNow)
        {
            user.IsLocked            = false;
            user.FailedLoginAttempts = 0;
            await _users.UpdateAsync(user);
        }

        if (user.IsLocked)
        {
            await WriteAuditAsync(user.Id, "LockedAccountLoginAttempt", "User", user.Id.ToString(),
                $"Login blocked — account locked until {user.LockedUntil:u}.", ipAddress);
            throw new UnauthorizedAccessException(
                $"Account is locked. Try again after {user.LockedUntil:HH:mm 'UTC'}.");
        }

        if (!_hasher.Verify(dto.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.IsLocked   = true;
                user.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                await _users.UpdateAsync(user);
                await WriteAuditAsync(user.Id, "AccountLocked", "User", user.Id.ToString(),
                    $"Account locked after {MaxFailedAttempts} failed attempts.", ipAddress);
                throw new UnauthorizedAccessException(
                    $"Too many failed attempts. Account locked for {LockoutMinutes} minutes.");
            }

            await _users.UpdateAsync(user);
            await WriteAuditAsync(user.Id, "FailedLogin", "User", user.Id.ToString(),
                $"Failed login attempt {user.FailedLoginAttempts}/{MaxFailedAttempts}.", ipAddress);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Successful login — reset failure counter
        user.FailedLoginAttempts = 0;
        user.IsLocked            = false;
        await _users.UpdateAsync(user);

        var fullName = user switch
        {
            GuestUser g => $"{g.FirstName} {g.LastName}",
            StaffUser s => $"{s.FirstName} {s.LastName}",
            _           => user.Email,
        };

        var requiresPasswordChange =
            user.Role is UserRole.Admin or UserRole.HotelManager &&
            (DateTime.UtcNow - user.LastPasswordChange).TotalDays >= PasswordExpiryDays;

        var (token, expiresAt) = _jwt.GenerateToken(user.Id, user.Email, user.Role.ToString());

        await WriteAuditAsync(user.Id, "Login", "User", user.Id.ToString(), "Successful login.", ipAddress);

        return new LoginResponseDto
        {
            Token                  = token,
            ExpiresAt              = expiresAt,
            UserId                 = user.Id,
            Role                   = user.Role.ToString(),
            FullName               = fullName,
            RequiresPasswordChange = requiresPasswordChange,
        };
    }

    // ── Register (Guest self-registration) ────────────────────────────────────

    public async Task<LoginResponseDto> RegisterGuestAsync(RegisterGuestDto dto, string ipAddress)
    {
        PasswordPolicy.Validate(dto.Password);

        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("An account with this email already exists.");

        var guest = new GuestUser
        {
            Email              = dto.Email.ToLowerInvariant(),
            PasswordHash       = _hasher.Hash(dto.Password),
            Role               = UserRole.Guest,
            FirstName          = dto.FirstName,
            LastName           = dto.LastName,
            Phone              = dto.Phone,
            Address            = dto.Address,
            LastPasswordChange = DateTime.UtcNow,
            CreatedAt          = DateTime.UtcNow,
        };

        await _users.AddAsync(guest);
        await WriteAuditAsync(guest.Id, "Register", "User", guest.Id.ToString(),
            $"New guest account registered: {guest.Email}.", ipAddress);

        var (token, expiresAt) = _jwt.GenerateToken(guest.Id, guest.Email, guest.Role.ToString());

        return new LoginResponseDto
        {
            Token    = token,
            ExpiresAt = expiresAt,
            UserId   = guest.Id,
            Role     = guest.Role.ToString(),
            FullName = $"{guest.FirstName} {guest.LastName}",
        };
    }

    // ── Change Password ───────────────────────────────────────────────────────

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto, string ipAddress)
    {
        var user = await _users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        if (!_hasher.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        PasswordPolicy.Validate(dto.NewPassword);

        if (_hasher.Verify(dto.NewPassword, user.PasswordHash))
            throw new InvalidOperationException("New password must be different from the current password.");

        user.PasswordHash       = _hasher.Hash(dto.NewPassword);
        user.LastPasswordChange = DateTime.UtcNow;
        await _users.UpdateAsync(user);

        await WriteAuditAsync(userId, "ChangePassword", "User", userId.ToString(),
            "Password changed successfully.", ipAddress);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Task WriteAuditAsync(int? userId, string action, string entityType,
        string entityId, string details, string ipAddress) =>
        _auditLogs.AddAsync(new AuditLog
        {
            UserId     = userId,
            Action     = action,
            EntityType = entityType,
            EntityId   = entityId,
            Details    = details,
            IpAddress  = ipAddress,
            Timestamp  = DateTime.UtcNow,
        });
}
