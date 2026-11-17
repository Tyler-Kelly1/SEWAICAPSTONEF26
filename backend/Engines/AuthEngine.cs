using System.Security.Claims;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class AuthEngine : IAuthEngine
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _context;

    public AuthEngine(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cleanEmail = dto.Email.Trim().ToLower();
        var cleanUsername = dto.UserName.Trim();

        var existingByEmail = await _userManager.FindByEmailAsync(cleanEmail);
        if (existingByEmail != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An account with email '{cleanEmail}' already exists."
            };
        }

        var existingByName = await _userManager.FindByNameAsync(cleanUsername);
        if (existingByName != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Username '{cleanUsername}' is already taken."
            };
        }

        var user = new ApplicationUser
        {
            Id = cleanUsername, // Keep username as stable Id for seamless foreign-key session linking
            UserName = cleanUsername,
            Email = cleanEmail
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Registration failed: {errors}"
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "Account registered successfully.",
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cleanEmail = dto.Email.Trim().ToLower();
        var user = await _userManager.FindByEmailAsync(cleanEmail);

        if (user == null)
        {
            // Also allow logging in with username directly in the email input
            user = await _userManager.FindByNameAsync(cleanEmail);
        }

        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        await _signInManager.SignInAsync(user, isPersistent: true);

        // Append a client-accessible session indicator cookie for frontend inactivity checking
        httpContext.Response.Cookies.Append("overload_auth_active", "true", new CookieOptions
        {
            HttpOnly = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(24)
        });

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        await _signInManager.SignOutAsync();
        httpContext.Response.Cookies.Delete("overload_auth_active");
        httpContext.Response.Cookies.Delete("OverloadAuth");
    }

    public async Task<AuthUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
        {
            return null;
        }

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            // Fallback by name claim
            var name = principal.Identity.Name;
            if (!string.IsNullOrEmpty(name))
            {
                user = await _userManager.FindByNameAsync(name);
            }
        }

        if (user == null) return null;

        return new AuthUserDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public string? GetAuthenticatedUserId(ClaimsPrincipal principal)
    {
        if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
        {
            return null;
        }

        var id = _userManager.GetUserId(principal);
        if (!string.IsNullOrEmpty(id))
        {
            return id;
        }

        return principal.Identity.Name;
    }

    public bool ValidateUserAccess(ClaimsPrincipal principal, string? requestedUserId)
    {
        if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
        {
            return false;
        }

        var authId = GetAuthenticatedUserId(principal);
        if (string.IsNullOrEmpty(authId))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(requestedUserId))
        {
            return true;
        }

        return string.Equals(authId, requestedUserId, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(principal.Identity.Name, requestedUserId, StringComparison.OrdinalIgnoreCase);
    }
}
