using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "User ID is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "User ID must be between 1 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "User ID can only contain alphanumeric characters, underscores, and hyphens.")]
    public string UserId { get; set; } = string.Empty;
}

public class UserCheckResultDto
{
    public string UserId { get; set; } = string.Empty;
    public bool Exists { get; set; }
}

public class UserDetailDto
{
    public string UserId { get; set; } = string.Empty;
}
