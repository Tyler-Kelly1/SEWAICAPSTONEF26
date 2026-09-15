namespace Backend.Models;

public class Session
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public int ExecutedWorkoutId { get; set; }
    public Workout ExecutedWorkout { get; set; } = null!;

    public DateTime SessionDate { get; set; }

    public TimeSpan Duration => ExecutedWorkout != null ? (ExecutedWorkout.EndTime - ExecutedWorkout.StartTime) : TimeSpan.Zero;
}
