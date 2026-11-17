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

    public async Task<IEnumerable<UserDto>> GetUsersWithSessionsAsync(string? userId = null)
    {
        IQueryable<ApplicationUser> query = _context.Users
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
                            .ThenInclude(st => st.Template);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(u => u.Id == userId || u.UserName == userId);
        }

        var users = await query.ToListAsync();
        return users.Select(u => MapToUserDto(u));
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsAsync(string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Enumerable.Empty<SessionDto>();
        }

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
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SessionDate);

        var sessions = await query.ToListAsync();
        return sessions.Select(MapToSessionDto);
    }

    public async Task<SessionDto?> GetSessionByIdAsync(int id, string? userId = null)
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
                        .ThenInclude(st => st.Template);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(s => s.UserId == userId);
        }

        var session = await query.FirstOrDefaultAsync(s => s.Id == id);
        return session != null ? MapToSessionDto(session) : null;
    }

    public async Task<SessionDto> CreateSessionAsync(CreateSessionDto dto)
    {
        if (dto.EndTime < dto.StartTime)
        {
            throw new ArgumentException("EndTime cannot be earlier than StartTime.");
        }

        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            throw new ArgumentException("User ID is required to record a session.");
        }

        // Validate that user exists in database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId || u.UserName == dto.UserId);
        if (user == null)
        {
            throw new ArgumentException($"User '{dto.UserId}' does not exist.");
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
            UserId = user.Id,
            ExecutedWorkout = workout,
            SessionDate = DateTime.UtcNow
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return MapToSessionDto(session);
    }

    public async Task<IEnumerable<WorkoutTemplateDto>> GetWorkoutTemplatesAsync(string? userId = null)
    {
        IQueryable<Workout_Template> query = _context.WorkoutTemplates
            .Include(wt => wt.ExerciseTemplates)
                .ThenInclude(et => et.SetTemplates);

        // Allow access only to:
        // 1. The global template ("Power Upper Body Workout", "Upper Body Power Template", or "Upper Body Template" with UserId == null)
        // 2. Custom templates created by the authenticated user
        query = query.Where(wt =>
            (wt.UserId == null && (wt.Workout_Name == "Power Upper Body Workout" || wt.Workout_Name == "Upper Body Power Template" || wt.Workout_Name == "Upper Body Template")) ||
            (!string.IsNullOrWhiteSpace(userId) && wt.UserId == userId));

        var templates = await query.ToListAsync();
        return templates.Select(MapToWorkoutTemplateDto)!;
    }

    public async Task<WorkoutTemplateDto?> GetWorkoutTemplateByIdAsync(int id, string? userId = null)
    {
        IQueryable<Workout_Template> query = _context.WorkoutTemplates
            .Include(wt => wt.ExerciseTemplates)
                .ThenInclude(et => et.SetTemplates);

        query = query.Where(wt => wt.Id == id && (
            (wt.UserId == null && (wt.Workout_Name == "Power Upper Body Workout" || wt.Workout_Name == "Upper Body Power Template" || wt.Workout_Name == "Upper Body Template")) ||
            (!string.IsNullOrWhiteSpace(userId) && wt.UserId == userId)));

        var template = await query.FirstOrDefaultAsync();
        return template != null ? MapToWorkoutTemplateDto(template) : null;
    }

    public async Task<WorkoutTemplateDto> CreateWorkoutTemplateAsync(CreateWorkoutTemplateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Workout_Name))
        {
            throw new ArgumentException("Workout template name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            throw new ArgumentException("User ID is required to create a workout template.");
        }

        var trimmedName = dto.Workout_Name.Trim();
        var trimmedUserId = dto.UserId.Trim();

        if (trimmedName.Equals("Power Upper Body Workout", StringComparison.OrdinalIgnoreCase) ||
            trimmedName.Equals("Upper Body Power Template", StringComparison.OrdinalIgnoreCase) ||
            trimmedName.Equals("Upper Body Template", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Cannot create a template with the reserved global template name.");
        }

        var exists = await _context.WorkoutTemplates.AnyAsync(wt =>
            wt.UserId == trimmedUserId && wt.Workout_Name.ToLower() == trimmedName.ToLower());
        if (exists)
        {
            throw new ArgumentException($"A workout template named '{trimmedName}' already exists.");
        }

        var workoutTemplate = new Workout_Template
        {
            Workout_Name = trimmedName,
            UserId = trimmedUserId
        };
        _context.WorkoutTemplates.Add(workoutTemplate);
        await _context.SaveChangesAsync();

        if (dto.ExerciseTemplates != null && dto.ExerciseTemplates.Count > 0)
        {
            foreach (var etDto in dto.ExerciseTemplates)
            {
                var exerciseTemplate = new Exercise_Template
                {
                    Exercise_Name = etDto.Exercise_Name.Trim(),
                    Min_Set = etDto.Min_Set > 0 ? etDto.Min_Set : 3,
                    Max_Set = etDto.Max_Set > 0 ? etDto.Max_Set : 5,
                    Weight_Step = etDto.Weight_Step > 0 ? etDto.Weight_Step : 0.05m,
                    Volume_Step = etDto.Volume_Step > 0 ? etDto.Volume_Step : 1,
                    WorkoutTemplateId = workoutTemplate.Id,
                    SetTemplates = etDto.SetTemplates?.Select(st => new Set_Template
                    {
                        Failure_Set = st.Failure_Set,
                        Min_Reps = st.Min_Reps,
                        Max_Reps = st.Max_Reps
                    }).ToList() ?? new List<Set_Template>()
                };
                _context.ExerciseTemplates.Add(exerciseTemplate);
            }
            await _context.SaveChangesAsync();
        }

        var created = await _context.WorkoutTemplates
            .Include(wt => wt.ExerciseTemplates)
                .ThenInclude(et => et.SetTemplates)
            .FirstAsync(wt => wt.Id == workoutTemplate.Id);

        return MapToWorkoutTemplateDto(created)!;
    }

    public async Task<IEnumerable<ExerciseTemplateDto>> GetExerciseTemplatesAsync(string? userId = null)
    {
        IQueryable<Exercise_Template> query = _context.ExerciseTemplates
            .Include(et => et.SetTemplates)
            .Include(et => et.WorkoutTemplate);

        query = query.Where(et =>
            (et.WorkoutTemplate != null && et.WorkoutTemplate.UserId == null && (et.WorkoutTemplate.Workout_Name == "Power Upper Body Workout" || et.WorkoutTemplate.Workout_Name == "Upper Body Power Template" || et.WorkoutTemplate.Workout_Name == "Upper Body Template")) ||
            (!string.IsNullOrWhiteSpace(userId) && (et.WorkoutTemplate == null || et.WorkoutTemplate.UserId == userId)));

        var templates = await query.ToListAsync();
        return templates.Select(MapToExerciseTemplateDto)!;
    }

    public async Task<ExerciseTemplateDto> CreateExerciseTemplateAsync(CreateExerciseTemplateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Exercise_Name))
        {
            throw new ArgumentException("Exercise name is required.");
        }

        var exerciseTemplate = new Exercise_Template
        {
            Exercise_Name = dto.Exercise_Name.Trim(),
            Min_Set = dto.Min_Set > 0 ? dto.Min_Set : 3,
            Max_Set = dto.Max_Set > 0 ? dto.Max_Set : 5,
            Weight_Step = dto.Weight_Step > 0 ? dto.Weight_Step : 0.05m,
            Volume_Step = dto.Volume_Step > 0 ? dto.Volume_Step : 1,
            WorkoutTemplateId = dto.WorkoutTemplateId,
            SetTemplates = dto.SetTemplates?.Select(st => new Set_Template
            {
                Failure_Set = st.Failure_Set,
                Min_Reps = st.Min_Reps,
                Max_Reps = st.Max_Reps
            }).ToList() ?? new List<Set_Template>()
        };

        _context.ExerciseTemplates.Add(exerciseTemplate);
        await _context.SaveChangesAsync();

        return MapToExerciseTemplateDto(exerciseTemplate)!;
    }

    private static UserDto MapToUserDto(ApplicationUser user)
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
            UserId = template.UserId,
            Workout_Name = template.Workout_Name,
            ExerciseTemplates = template.ExerciseTemplates?.Select(et => new ExerciseTemplateDto
            {
                Id = et.Id,
                Exercise_Name = et.Exercise_Name,
                Max_Set = et.Max_Set,
                Min_Set = et.Min_Set,
                Weight_Step = et.Weight_Step,
                Volume_Step = et.Volume_Step,
                SetTemplates = et.SetTemplates?.Select(st => new SetTemplateDto
                {
                    Id = st.Id,
                    Failure_Set = st.Failure_Set,
                    Max_Reps = st.Max_Reps,
                    Min_Reps = st.Min_Reps
                }).ToList() ?? new List<SetTemplateDto>()
            }).ToList() ?? new List<ExerciseTemplateDto>()
        };
    }
}
