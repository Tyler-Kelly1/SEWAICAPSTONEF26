namespace Backend.Models.Templates;

public class Workout_Template
{
    public int Id { get; set; }
    public string Workout_Name { get; set; } = string.Empty;
    public List<Exercise_Template> ExerciseTemplates { get; set; } = new();
}
