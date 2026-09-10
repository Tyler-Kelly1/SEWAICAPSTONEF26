using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateTestItemDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Value is required.")]
    [StringLength(500, ErrorMessage = "Value cannot exceed 500 characters.")]
    public string Value { get; set; } = string.Empty;
}


