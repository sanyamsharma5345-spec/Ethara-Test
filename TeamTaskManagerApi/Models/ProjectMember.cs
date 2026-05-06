using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskManagerApi.Models;

public class ProjectMember
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey("Project")]
    public Guid ProjectId { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Project? Project { get; set; }
    public virtual User? User { get; set; }
}
