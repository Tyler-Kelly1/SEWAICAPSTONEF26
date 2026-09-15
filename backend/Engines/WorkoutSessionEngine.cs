using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class WorkoutSessionEngine : IWorkoutSessionEngine
{
    private readonly AppDbContext _context;

    public WorkoutSessionEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync()
    {
        var users = await _context.Users
            .Include(u => u.Sessions)
                .ThenInclude(s => s.ExecutedWorkout)
                    .ThenInclude(w => w.Exercises)
                        .ThenInclude(e => e.Sets)
            .ToListAsync();

        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null)
    {
        IQueryable<Session> query = _context.Sessions
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Sets)
            .OrderByDescending(s => s.SessionDate);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(s => s.UserId == userId);
        }

        var sessions = await query.ToListAsync();
        return sessions.Select(MapToSessionDto);
    }

    public async Task<SessionDto?> GetSessionByIdAsync(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(s => s.Id == id);

        return session != null ? MapToSessionDto(session) : null;
    }

    public async Task<SessionDto> CreateSessionAsync(CreateSessionDto dto)
    {
        if (dto.EndTime < dto.StartTime)
        {
            throw new ArgumentException("EndTime cannot be earlier than StartTime.");
        }

        // Ensure user exists
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            user = new User { UserId = dto.UserId };
            _context.Users.Add(user);
        }

        var workout = new Workout
        {
            WorkoutName = dto.WorkoutName,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Exercises = dto.Exercises.Select(e => new Exercise
            {
                ExerciseName = e.ExerciseName,
                Sets = e.Sets.Select(s => new Set
                {
                    Weight = s.Weight,
                    Reps = s.Reps
                }).ToList()
            }).ToList()
        };

        var session = new Session
        {
            UserId = dto.UserId,
            ExecutedWorkout = workout,
            SessionDate = DateTime.UtcNow
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return MapToSessionDto(session);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Sessions = user.Sessions?.Select(MapToSessionDto).ToList() ?? new List<SessionDto>()
        };
    }

    private static SessionDto MapToSessionDto(Session session)
    {
        return new SessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            SessionDate = session.SessionDate,
            Duration = session.Duration,
            ExecutedWorkout = session.ExecutedWorkout != null ? new WorkoutDto
            {
                Id = session.ExecutedWorkout.Id,
                WorkoutName = session.ExecutedWorkout.WorkoutName,
                StartTime = session.ExecutedWorkout.StartTime,
                EndTime = session.ExecutedWorkout.EndTime,
                Exercises = session.ExecutedWorkout.Exercises?.Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    ExerciseName = e.ExerciseName,
                    Sets = e.Sets?.Select(s => new SetDto
                    {
                        Id = s.Id,
                        Weight = s.Weight,
                        Reps = s.Reps
                    }).ToList() ?? new List<SetDto>()
                }).ToList() ?? new List<ExerciseDto>()
            } : new WorkoutDto()
        };
    }
}
