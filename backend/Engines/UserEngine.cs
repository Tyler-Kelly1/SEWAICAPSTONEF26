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

        var normalized = userId.Trim().ToLower();
        return await _context.Users.AnyAsync(u => u.UserId.ToLower() == normalized);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var normalized = userId.Trim().ToLower();
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToLower() == normalized);
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

        var user = new User
        {
            UserId = cleanUserId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.OrderBy(u => u.UserId).ToListAsync();
    }
}
