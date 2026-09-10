using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("TEST_TABLE")]
public class TestItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("VALUE")]
    public string Value { get; set; } = string.Empty;

    [Column("INSERT_TIME_STAMP")]
    public DateTime InsertTimeStamp { get; set; } = DateTime.UtcNow;
}
