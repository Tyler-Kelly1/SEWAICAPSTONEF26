using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Models.Templates;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class WorkoutSessionEngine : IWorkoutSessionEngine
{
    private readonly AppDbContext _context;
    private readonly IOverloadEngine _overloadEngine;

    public WorkoutSessionEngine(AppDbContext context, IOverloadEngine overloadEngine)
    {
        _context = context;
        _overloadEngine = overloadEngine;
    }

    public async Task<ExerciseGoalResponseDto> GetGoalSetsForExerciseAsync(ExerciseGoalRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Exercise_Template? exerciseTemp = null;

        if (dto.ExerciseTemplateId.HasValue)
        {
            exerciseTemp = await _context.ExerciseTemplates
                .Include(et => et.SetTemplates)
                .FirstOrDefaultAsync(et => et.Id == dto.ExerciseTemplateId.Value);
        }

        if (exerciseTemp == null && !string.IsNullOrWhiteSpace(dto.ExerciseName))
        {
            exerciseTemp = await _context.ExerciseTemplates
                .Include(et => et.SetTemplates)
                .FirstOrDefaultAsync(et => et.Exercise_Name.ToLower() == dto.ExerciseName.Trim().ToLower());
        }

        if (exerciseTemp == null)
        {
            exerciseTemp = new Exercise_Template
            {
                Exercise_Name = dto.ExerciseName,
                Weight_Step = 0.05m,
                Volume_Step = 1,
                SetTemplates = new List<Set_Template>()
            };
        }

        var goalSets = new List<GoalSetDto>();
        int setCount = (dto.Sets != null && dto.Sets.Count > 0)
            ? dto.Sets.Count
            : (exerciseTemp.SetTemplates != null && exerciseTemp.SetTemplates.Count > 0 ? exerciseTemp.SetTemplates.Count : 1);

        for (int i = 0; i < setCount; i++)
        {
            Set_Template? setTemp = null;

            if (exerciseTemp.SetTemplates != null && exerciseTemp.SetTemplates.Count > 0)
            {
                var orderedTemplates = exerciseTemp.SetTemplates.OrderBy(st => st.Id).ToList();
                setTemp = i < orderedTemplates.Count
                    ? orderedTemplates[i]
                    : orderedTemplates.Last();
            }

            if (setTemp == null)
            {
                setTemp = new Set_Template
                {
                    Min_Reps = 6,
                    Max_Reps = 10,
                    Failure_Set = false
                };
            }

            Set_Value? previousSetValue = await _overloadEngine.GetPreviousSetValueAsync(dto.ExerciseTemplateId ?? exerciseTemp.Id, dto.ExerciseName, i, dto.UserId);

            if (previousSetValue == null && dto.Sets != null && i < dto.Sets.Count)
            {
                var currentSet = dto.Sets[i];
                previousSetValue = new Set_Value
                {
                    Weight = currentSet.Weight,
                    Reps = currentSet.Reps
                };
            }

            previousSetValue ??= new Set_Value
            {
                Weight = 100,
                Reps = setTemp.Min_Reps
            };

            var calculatedGoal = _overloadEngine.GetGoalSet(exerciseTemp, setTemp, previousSetValue);

            goalSets.Add(new GoalSetDto
            {
                SetNumber = i + 1,
                Weight = calculatedGoal.Weight,
                Reps = calculatedGoal.Reps
            });
        }

        return new ExerciseGoalResponseDto
        {
            ExerciseName = dto.ExerciseName,
            ExerciseTemplateId = exerciseTemp.Id != 0 ? exerciseTemp.Id : dto.ExerciseTemplateId,
            GoalSets = goalSets
        };
    }

    public async Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync()
    {
        var users = await _context.Users
            .Include(u => u.Sessions)
                .ThenInclude(s => s.ExecutedWorkout)
                    .ThenInclude(w => w.Template)
            .Include(u => u.Sessions)
                .ThenInclude(s => s.ExecutedWorkout)
                    .ThenInclude(w => w.Exercises)
                        .ThenInclude(e => e.Template)
            .Include(u => u.Sessions)
                .ThenInclude(s => s.ExecutedWorkout)
                    .ThenInclude(w => w.Exercises)
                        .ThenInclude(e => e.Sets)
                            .ThenInclude(st => st.Template)
            .ToListAsync();

        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null)
    {
        IQueryable<Session> query = _context.Sessions
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Template)
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Template)
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Sets)
                        .ThenInclude(st => st.Template)
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
                .ThenInclude(w => w.Template)
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Template)
            .Include(s => s.ExecutedWorkout)
                .ThenInclude(w => w.Exercises)
                    .ThenInclude(e => e.Sets)
                        .ThenInclude(st => st.Template)
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

        var existingTemplateIds = await _context.ExerciseTemplates
            .Select(et => et.Id)
            .ToListAsync();

        var workout = new Workout
        {
            WorkoutName = dto.WorkoutName,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Exercises = dto.Exercises.Select(e =>
            {
                int? templateId = null;
                if (e.ExerciseTemplateId.HasValue && existingTemplateIds.Contains(e.ExerciseTemplateId.Value))
                {
                    templateId = e.ExerciseTemplateId.Value;
                }

                return new Exercise
                {
                    ExerciseName = e.ExerciseName,
                    ExerciseTemplateId = templateId,
                    Sets = e.Sets.Select(s => new Set
                    {
                        Weight = s.Weight,
                        Reps = s.Reps
                    }).ToList()
                };
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
                Template = MapToWorkoutTemplateDto(session.ExecutedWorkout.Template),
                Exercises = session.ExecutedWorkout.Exercises?.Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    ExerciseName = e.ExerciseName,
                    Template = MapToExerciseTemplateDto(e.Template),
                    Sets = e.Sets?.Select(s => new SetDto
                    {
                        Id = s.Id,
                        Weight = s.Weight,
                        Reps = s.Reps,
                        Template = MapToSetTemplateDto(s.Template)
                    }).ToList() ?? new List<SetDto>()
                }).ToList() ?? new List<ExerciseDto>()
            } : new WorkoutDto()
        };
    }

    private static SetTemplateDto? MapToSetTemplateDto(Set_Template? template)
    {
        if (template == null) return null;
        return new SetTemplateDto
        {
            Id = template.Id,
            Failure_Set = template.Failure_Set,
            Max_Reps = template.Max_Reps,
            Min_Reps = template.Min_Reps
        };
    }

    private static ExerciseTemplateDto? MapToExerciseTemplateDto(Exercise_Template? template)
    {
        if (template == null) return null;
        return new ExerciseTemplateDto
        {
            Id = template.Id,
            Exercise_Name = template.Exercise_Name,
            Max_Set = template.Max_Set,
            Min_Set = template.Min_Set,
            Weight_Step = template.Weight_Step,
            Volume_Step = template.Volume_Step,
            SetTemplates = template.SetTemplates?.Select(st => new SetTemplateDto
            {
                Id = st.Id,
                Failure_Set = st.Failure_Set,
                Max_Reps = st.Max_Reps,
                Min_Reps = st.Min_Reps
            }).ToList() ?? new List<SetTemplateDto>()
        };
    }

    private static WorkoutTemplateDto? MapToWorkoutTemplateDto(Workout_Template? template)
    {
        if (template == null) return null;
        return new WorkoutTemplateDto
        {
            Id = template.Id,
            Workout_Name = template.Workout_Name,
            ExerciseTemplates = template.ExerciseTemplates?.Select(et => new ExerciseTemplateDto
            {
                Id = et.Id,
                Exercise_Name = et.Exercise_Name,
                Max_Set = et.Max_Set,
                Min_Set = et.Min_Set,
                Weight_Step = et.Weight_Step,
                Volume_Step = et.Volume_Step
            }).ToList() ?? new List<ExerciseTemplateDto>()
        };
    }
}
