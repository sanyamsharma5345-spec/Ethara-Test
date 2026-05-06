using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Services;

namespace TeamTaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Member";
    }

    /// <summary>
    /// Get all tasks (Admin gets all, Member gets assigned tasks)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTasks()
    {
        var userRole = GetCurrentUserRole();
        
        if (userRole == "Admin")
        {
            var result = await _taskService.GetAllTasksAsync();
            return Ok(result);
        }
        else
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetUserTasksAsync(userId);
            return Ok(result);
        }
    }

    /// <summary>
    /// Create a new task (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminId = GetCurrentUserId();
        var result = await _taskService.CreateTaskAsync(adminId, dto);

        if (result == null)
            return BadRequest("Failed to create task");

        return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get task details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var result = await _taskService.GetTaskAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Get all tasks for a project
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectTasks(Guid projectId)
    {
        var result = await _taskService.GetProjectTasksAsync(projectId);
        return Ok(result);
    }

    /// <summary>
    /// Get all tasks assigned to current user
    /// </summary>
    [HttpGet("assigned-to-me")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.GetUserTasksAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Update task status
    /// </summary>
    [HttpPatch("{taskId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] UpdateTaskStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();
        var result = await _taskService.UpdateTaskStatusAsync(taskId, dto, userId, userRole);

        if (!result)
            return NotFound("Task not found or unauthorized");

        return Ok("Task status updated successfully");
    }

    /// <summary>
    /// Update task (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminId = GetCurrentUserId();
        var result = await _taskService.UpdateTaskAsync(id, dto, adminId);

        if (!result)
            return NotFound("Task not found or unauthorized");

        return Ok("Task updated successfully");
    }

    /// <summary>
    /// Delete task (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var adminId = GetCurrentUserId();
        var result = await _taskService.DeleteTaskAsync(id, adminId);

        if (!result)
            return NotFound("Task not found or unauthorized");

        return Ok("Task deleted successfully");
    }

    /// <summary>
    /// Get all overdue tasks
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueTasks()
    {
        var result = await _taskService.GetOverdueTasksAsync();
        return Ok(result);
    }
}
