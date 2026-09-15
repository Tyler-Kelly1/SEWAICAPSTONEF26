namespace Backend.Models;

public class Exercise
{
    public int Id { get; set; }
    public int WorkoutId { get; set; }
    public Workout? Workout { get; set; }

    public string ExerciseName { get; set; } = string.Empty;
    public List<Set> Sets { get; set; } = new();
}
