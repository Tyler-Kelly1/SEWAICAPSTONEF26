using Backend.DTOs;

namespace Backend.Engines;

public interface IWorkoutSessionEngine
{
    Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync();
    Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null);
    Task<SessionDto?> GetSessionByIdAsync(int id);
    Task<SessionDto> CreateSessionAsync(CreateSessionDto dto);
}
