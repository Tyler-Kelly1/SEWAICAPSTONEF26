using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Backend.Models.Templates;

namespace Backend.Models;

public class ApplicationUser : IdentityUser
{
    [NotMapped]
    public string UserId
    {
        get => UserName ?? Id;
        set => UserName = value;
    }

    public List<Session> Sessions { get; set; } = new();
    public List<Workout_Template> WorkoutTemplates { get; set; } = new();
}
