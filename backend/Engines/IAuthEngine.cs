using System.Security.Claims;
using Backend.DTOs;
using Microsoft.AspNetCore.Http;

namespace Backend.Engines;

public interface IAuthEngine
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);
    Task<AuthUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal);
    string? GetAuthenticatedUserId(ClaimsPrincipal principal);
    bool ValidateUserAccess(ClaimsPrincipal principal, string? requestedUserId);
}
