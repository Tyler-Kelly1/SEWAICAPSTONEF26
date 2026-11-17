using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class UserEngine : IUserEngine
{
    private readonly AppDbContext _context;

    public UserEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckUserExistsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var normalized = userId.Trim().ToUpper();
        return await _context.Users.AnyAsync(u => u.NormalizedUserName == normalized || u.UserName == userId.Trim() || u.Id == userId.Trim());
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var normalized = userId.Trim().ToUpper();
        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalized || u.UserName == userId.Trim() || u.Id == userId.Trim());
        return appUser != null ? new User { UserId = appUser.UserId } : null;
    }

    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.UserId))
        {
            throw new ArgumentException("User ID must be provided.", nameof(dto));
        }

        var cleanUserId = dto.UserId.Trim();
        var exists = await CheckUserExistsAsync(cleanUserId);
        if (exists)
        {
            throw new InvalidOperationException($"User with ID '{cleanUserId}' already exists.");
        }

        var appUser = new ApplicationUser
        {
            Id = cleanUserId,
            UserName = cleanUserId,
            NormalizedUserName = cleanUserId.ToUpper(),
            Email = $"{cleanUserId}@example.com",
            NormalizedEmail = $"{cleanUserId.ToUpper()}@EXAMPLE.COM"
        };

        _context.Users.Add(appUser);
        await _context.SaveChangesAsync();

        return new User { UserId = appUser.UserId };
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var appUsers = await _context.Users.OrderBy(u => u.UserName).ToListAsync();
        return appUsers.Select(u => new User { UserId = u.UserId });
    }
}
