using Backend.Models.Templates;

namespace Backend.Models;

public class Set : Set_Value
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int? SetTemplateId { get; set; }
    public Set_Template? Template { get; set; }
}
