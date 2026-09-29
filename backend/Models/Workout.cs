using Backend.Models.Templates;

namespace Backend.Models;

public class Workout
{
    public int Id { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public List<Exercise> Exercises { get; set; } = new();

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int? WorkoutTemplateId { get; set; }
    public Workout_Template? Template { get; set; }
}
