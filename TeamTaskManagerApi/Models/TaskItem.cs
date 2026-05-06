using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskManagerApi.Models;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public required string Status { get; set; } // Pending, InProgress, Completed

    public DateTime? DueDate { get; set; }

    [ForeignKey("AssignedTo")]
    public Guid? AssignedToId { get; set; }

    [Required]
    [ForeignKey("Project")]
    public Guid ProjectId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual User? AssignedToUser { get; set; }
    public virtual Project? Project { get; set; }

    public bool IsOverdue => Status != "Completed" && DueDate.HasValue && DueDate < DateTime.UtcNow;
}
