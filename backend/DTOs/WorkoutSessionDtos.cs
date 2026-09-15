using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class SetDto
{
    public int Id { get; set; }
    public int Weight { get; set; }
    public int Reps { get; set; }
}

public class ExerciseDto
{
    public int Id { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public List<SetDto> Sets { get; set; } = new();
}

public class WorkoutDto
{
    public int Id { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = new();
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
