namespace Backend.Models.Templates;

public class Exercise_Template
{
    public int Id { get; set; }
    public string Exercise_Name { get; set; } = string.Empty;
    public int Max_Set { get; set; }
    public int Min_Set { get; set; }
    public decimal Weight_Step { get; set; } = 0.05m;
    public int Volume_Step { get; set; } = 1;

    public int? WorkoutTemplateId { get; set; }
    public Workout_Template? WorkoutTemplate { get; set; }

    public List<Set_Template> SetTemplates { get; set; } = new();
}
