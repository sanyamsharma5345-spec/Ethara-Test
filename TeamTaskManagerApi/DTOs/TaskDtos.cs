namespace TeamTaskManagerApi.DTOs;

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedToId { get; set; }
    public required Guid ProjectId { get; set; }
}

public class UpdateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Status { get; set; } // Pending, InProgress, Completed
    public DateTime? DueDate { get; set; }
    public Guid? AssignedToId { get; set; }
}

public class UpdateTaskStatusDto
{
    public required string Status { get; set; } // Pending, InProgress, Completed
}

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Status { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsOverdue { get; set; }
}

public class DashboardStatsDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int OverdueTasks { get; set; }
}
