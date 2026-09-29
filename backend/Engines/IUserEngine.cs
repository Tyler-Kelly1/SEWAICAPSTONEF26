using Backend.DTOs;
using Backend.Models;

namespace Backend.Engines;

public interface IUserEngine
{
    Task<bool> CheckUserExistsAsync(string userId);
    Task<User?> GetUserByIdAsync(string userId);
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<IEnumerable<User>> GetAllUsersAsync();
}
