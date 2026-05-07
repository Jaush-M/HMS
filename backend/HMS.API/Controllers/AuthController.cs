// Author: Salaams
using System.Security.Claims;
using HMS.Application.DTOs.Auth;
using HMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Authenticates a user and returns a JWT bearer token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var ip       = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var response = await _authService.LoginAsync(dto, ip);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
    }

    /// <summary>Registers a new guest account and returns a JWT.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterGuestDto dto)
    {
        try
        {
            var ip       = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var response = await _authService.RegisterGuestAsync(dto, ip);
            return CreatedAtAction(null, response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(ex.Message);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Changes the authenticated user's password.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized("Invalid token — user ID claim missing.");

        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            await _authService.ChangePasswordAsync(userId, dto, ip);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (InvalidOperationException ex)   { return BadRequest(ex.Message); }
        catch (KeyNotFoundException ex)        { return NotFound(ex.Message); }
    }
}
