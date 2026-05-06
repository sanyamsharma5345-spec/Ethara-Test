using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Models;
using TeamTaskManagerApi.Repositories;

namespace TeamTaskManagerApi.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<TaskResponseDto?> CreateTaskAsync(Guid adminId, CreateTaskDto dto)
    {
        try
        {
            // Verify admin is creator of project
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
            if (project == null || project.CreatedById != adminId)
            {
                _logger.LogWarning("Unauthorized: Cannot create task in project");
                return null;
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = "Pending",
                DueDate = dto.DueDate,
                AssignedToId = dto.AssignedToId,
                ProjectId = dto.ProjectId
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task created: {TaskId} in project {ProjectId}", task.Id, dto.ProjectId);

            return MapToDto(task, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return null;
        }
    }

    public async Task<TaskResponseDto?> GetTaskAsync(Guid taskId)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return null;
            }

            return MapToDto(task, task.AssignedToUser?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task");
            return null;
        }
    }

    public async Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(Guid projectId)
    {
        try
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
            return tasks.Select(t => MapToDto(t, t.AssignedToUser?.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project tasks");
            return [];
        }
    }

    public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(Guid userId)
    {
        try
        {
            var tasks = await _taskRepository.GetByAssignedToAsync(userId);
            return tasks.Select(t => MapToDto(t, t.AssignedToUser?.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user tasks");
            return [];
        }
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(t => MapToDto(t, t.AssignedToUser?.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            return [];
        }
    }

    public async Task<bool> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto dto, Guid userId, string userRole)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return false;
            }

            // Only assigned user or admin can update status
            if (userRole != "Admin" && task.AssignedToId != userId)
            {
                _logger.LogWarning("Unauthorized: Cannot update task status");
                return false;
            }

            task.Status = dto.Status;
            if (dto.Status == "Completed")
            {
                task.CompletedAt = DateTime.UtcNow;
            }

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task status updated: {TaskId} to {Status}", taskId, dto.Status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status");
            return false;
        }
    }

    public async Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto, Guid adminId)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return false;
            }

            // Verify admin is creator of project
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null || project.CreatedById != adminId)
            {
                _logger.LogWarning("Unauthorized: Cannot update task");
                return false;
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.DueDate = dto.DueDate;
            task.AssignedToId = dto.AssignedToId;

            if (dto.Status == "Completed" && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
            }

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task updated: {TaskId}", taskId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task");
            return false;
        }
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, Guid adminId)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return false;
            }

            // Verify admin is creator of project
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null || project.CreatedById != adminId)
            {
                _logger.LogWarning("Unauthorized: Cannot delete task");
                return false;
            }

            await _taskRepository.DeleteAsync(taskId);
            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("Task deleted: {TaskId}", taskId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task");
            return false;
        }
    }

    public async Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync()
    {
        try
        {
            var tasks = await _taskRepository.GetOverdueTasksAsync();
            return tasks.Select(t => MapToDto(t, t.AssignedToUser?.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue tasks");
            return [];
        }
    }

    private static TaskResponseDto MapToDto(TaskItem task, string? assignedToName)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            AssignedToId = task.AssignedToId,
            AssignedToName = assignedToName,
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            IsOverdue = task.IsOverdue
        };
    }
}
