using Backend.DTOs;

namespace Backend.Engines;

public interface IWorkoutSessionEngine
{
    Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync(string? userId = null);
    Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null);
    Task<SessionDto?> GetSessionByIdAsync(int id, string? userId = null);
    Task<SessionDto> CreateSessionAsync(CreateSessionDto dto);
    Task<ExerciseGoalResponseDto> GetGoalSetsForExerciseAsync(ExerciseGoalRequestDto dto);
    Task<IEnumerable<WorkoutTemplateDto>> GetWorkoutTemplatesAsync(string? userId = null);
    Task<WorkoutTemplateDto?> GetWorkoutTemplateByIdAsync(int id, string? userId = null);
    Task<WorkoutTemplateDto> CreateWorkoutTemplateAsync(CreateWorkoutTemplateDto dto);
    Task<IEnumerable<ExerciseTemplateDto>> GetExerciseTemplatesAsync(string? userId = null);
    Task<ExerciseTemplateDto> CreateExerciseTemplateAsync(CreateExerciseTemplateDto dto);
}
