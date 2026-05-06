namespace TeamTaskManagerApi.DTOs;

public class CreateProjectDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateProjectDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserResponseDto>? Members { get; set; }
}

public class AddProjectMemberDto
{
    public Guid UserId { get; set; }
}
