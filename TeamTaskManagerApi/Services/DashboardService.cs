using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Repositories;

namespace TeamTaskManagerApi.Services;

public class DashboardService : IDashboardService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        ILogger<DashboardService> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        try
        {
            var allTasks = await _taskRepository.GetAllAsync();

            return new DashboardStatsDto
            {
                TotalTasks = allTasks.Count(),
                CompletedTasks = allTasks.Count(t => t.Status == "Completed"),
                PendingTasks = allTasks.Count(t => t.Status == "Pending"),
                InProgressTasks = allTasks.Count(t => t.Status == "InProgress"),
                OverdueTasks = allTasks.Count(t => t.IsOverdue)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return new DashboardStatsDto();
        }
    }

    public async Task<DashboardStatsDto> GetProjectStatsAsync(Guid projectId)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                _logger.LogWarning("Project not found: {ProjectId}", projectId);
                return new DashboardStatsDto();
            }

            var projectTasks = await _taskRepository.GetByProjectIdAsync(projectId);

            return new DashboardStatsDto
            {
                TotalTasks = projectTasks.Count(),
                CompletedTasks = projectTasks.Count(t => t.Status == "Completed"),
                PendingTasks = projectTasks.Count(t => t.Status == "Pending"),
                InProgressTasks = projectTasks.Count(t => t.Status == "InProgress"),
                OverdueTasks = projectTasks.Count(t => t.IsOverdue)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project stats");
            return new DashboardStatsDto();
        }
    }

    public async Task<DashboardStatsDto> GetUserStatsAsync(Guid userId)
    {
        try
        {
            var userTasks = await _taskRepository.GetByAssignedToAsync(userId);

            return new DashboardStatsDto
            {
                TotalTasks = userTasks.Count(),
                CompletedTasks = userTasks.Count(t => t.Status == "Completed"),
                PendingTasks = userTasks.Count(t => t.Status == "Pending"),
                InProgressTasks = userTasks.Count(t => t.Status == "InProgress"),
                OverdueTasks = userTasks.Count(t => t.IsOverdue)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user stats");
            return new DashboardStatsDto();
        }
    }
}
