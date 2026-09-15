namespace Backend.Models;

public class Workout
{
    public int Id { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = new();

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
