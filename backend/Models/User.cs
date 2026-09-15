using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class User
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    public List<Session> Sessions { get; set; } = new();
}
