using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class SetTemplateDto
{
    public int Id { get; set; }
    public bool Failure_Set { get; set; }
    public int Max_Reps { get; set; }
    public int Min_Reps { get; set; }
}

public class ExerciseTemplateDto
{
    public int Id { get; set; }
    public string Exercise_Name { get; set; } = string.Empty;
    public int Max_Set { get; set; }
    public int Min_Set { get; set; }
    public decimal Weight_Step { get; set; }
    public int Volume_Step { get; set; }
    public List<SetTemplateDto> SetTemplates { get; set; } = new();
}

public class WorkoutTemplateDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Workout_Name { get; set; } = string.Empty;
    public List<ExerciseTemplateDto> ExerciseTemplates { get; set; } = new();
}

public class SetDto
{
    public int Id { get; set; }
    public int Weight { get; set; }
    public int Reps { get; set; }
    public SetTemplateDto? Template { get; set; }
}

public class ExerciseDto
{
    public int Id { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public List<SetDto> Sets { get; set; } = new();
    public ExerciseTemplateDto? Template { get; set; }
}

public class WorkoutDto
{
    public int Id { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = new();
    public WorkoutTemplateDto? Template { get; set; }
}

public class SessionDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public TimeSpan Duration { get; set; }
    public WorkoutDto ExecutedWorkout { get; set; } = null!;
}

public class UserDto
{
    public string UserId { get; set; } = string.Empty;
    public List<SessionDto> Sessions { get; set; } = new();
}

public class CreateSetDto
{
    [Range(0, 5000, ErrorMessage = "Weight must be between 0 and 5000 lbs.")]
    public int Weight { get; set; }

    [Range(1, 1000, ErrorMessage = "Reps must be between 1 and 1000.")]
    public int Reps { get; set; }
}

public class CreateExerciseDto
{
    public int? ExerciseTemplateId { get; set; }

    [Required(ErrorMessage = "Exercise name is required.")]
    [StringLength(100, ErrorMessage = "Exercise name cannot exceed 100 characters.")]
    public string ExerciseName { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one set is required per exercise.")]
    public List<CreateSetDto> Sets { get; set; } = new();
}

public class CreateSessionDto
{
    [Required(ErrorMessage = "User ID is required.")]
    [StringLength(50, ErrorMessage = "User ID cannot exceed 50 characters.")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Workout name is required.")]
    [StringLength(100, ErrorMessage = "Workout name cannot exceed 100 characters.")]
    public string WorkoutName { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one exercise is required for a workout.")]
    public List<CreateExerciseDto> Exercises { get; set; } = new();
}

public class ExerciseGoalRequestDto
{
    public string? UserId { get; set; }
    public int? ExerciseTemplateId { get; set; }

    [Required(ErrorMessage = "Exercise name is required.")]
    [StringLength(100, ErrorMessage = "Exercise name cannot exceed 100 characters.")]
    public string ExerciseName { get; set; } = string.Empty;

    public List<CreateSetDto>? Sets { get; set; } = new();
}

public class GoalSetDto
{
    public int SetNumber { get; set; }
    public int Weight { get; set; }
    public int Reps { get; set; }
}

public class ExerciseGoalResponseDto
{
    public string ExerciseName { get; set; } = string.Empty;
    public int? ExerciseTemplateId { get; set; }
    public List<GoalSetDto> GoalSets { get; set; } = new();
}

public class CreateSetTemplateDto
{
    public bool Failure_Set { get; set; }

    [Range(1, 1000, ErrorMessage = "Max reps must be between 1 and 1000.")]
    public int Max_Reps { get; set; } = 10;

    [Range(1, 1000, ErrorMessage = "Min reps must be between 1 and 1000.")]
    public int Min_Reps { get; set; } = 6;
}

public class CreateExerciseTemplateDto
{
    public int? Id { get; set; }

    public string? UserId { get; set; }

    [Required(ErrorMessage = "Exercise name is required.")]
    [StringLength(100, ErrorMessage = "Exercise name cannot exceed 100 characters.")]
    public string Exercise_Name { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Min sets must be between 1 and 100.")]
    public int Min_Set { get; set; } = 3;

    [Range(1, 100, ErrorMessage = "Max sets must be between 1 and 100.")]
    public int Max_Set { get; set; } = 5;

    [Range(0.001, 1.0, ErrorMessage = "Weight step must be between 0.001 and 1.0.")]
    public decimal Weight_Step { get; set; } = 0.05m;

    [Range(1, 100, ErrorMessage = "Volume step must be between 1 and 100.")]
    public int Volume_Step { get; set; } = 1;

    public int? WorkoutTemplateId { get; set; }

    public List<CreateSetTemplateDto>? SetTemplates { get; set; } = new();
}

public class CreateWorkoutTemplateDto
{
    public string? UserId { get; set; }

    [Required(ErrorMessage = "Workout template name is required.")]
    [StringLength(100, ErrorMessage = "Workout template name cannot exceed 100 characters.")]
    public string Workout_Name { get; set; } = string.Empty;

    public List<CreateExerciseTemplateDto>? ExerciseTemplates { get; set; } = new();
}


