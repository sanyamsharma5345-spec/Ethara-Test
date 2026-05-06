using TeamTaskManagerApi.DTOs;

namespace TeamTaskManagerApi.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> RegisterAsync(RegisterUserDto dto);
    Task<LoginResponseDto?> LoginAsync(LoginUserDto dto);
    string GenerateJwtToken(Guid userId, string userEmail, string userRole);
}

public interface IProjectService
{
    Task<ProjectResponseDto?> CreateProjectAsync(Guid userId, CreateProjectDto dto);
    Task<ProjectResponseDto?> GetProjectAsync(Guid projectId);
    Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(Guid userId);
    Task<bool> AddMemberAsync(Guid projectId, Guid adminId, AddProjectMemberDto dto);
    Task<IEnumerable<UserResponseDto>> GetProjectMembersAsync(Guid projectId);
    Task<bool> DeleteProjectAsync(Guid projectId, Guid adminId);
}

public interface ITaskService
{
    Task<TaskResponseDto?> CreateTaskAsync(Guid adminId, CreateTaskDto dto);
    Task<TaskResponseDto?> GetTaskAsync(Guid taskId);
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(Guid projectId);
    Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId);
    Task<bool> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto dto, Guid userId, string userRole);
    Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto, Guid adminId);
    Task<bool> DeleteTaskAsync(Guid taskId, Guid adminId);
    Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync();
}

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<DashboardStatsDto> GetProjectStatsAsync(Guid projectId);
    Task<DashboardStatsDto> GetUserStatsAsync(Guid userId);
}
