using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Services;

namespace TeamTaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
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
    /// Create a new project (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var result = await _projectService.CreateProjectAsync(userId, dto);

        if (result == null)
            return BadRequest("Failed to create project");

        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get project details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var result = await _projectService.GetProjectAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Get all projects for current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProjects()
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.GetUserProjectsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Add member to project (Admin only)
    /// </summary>
    [HttpPost("{projectId}/members")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddMember(Guid projectId, [FromBody] AddProjectMemberDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.AddMemberAsync(projectId, userId, dto);

        if (!result)
            return NotFound("Project not found or unauthorized");

        return Ok("Member added successfully");
    }

    /// <summary>
    /// Get project members
    /// </summary>
    [HttpGet("{projectId}/members")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectMembers(Guid projectId)
    {
        var result = await _projectService.GetProjectMembersAsync(projectId);
        return Ok(result);
    }

    /// <summary>
    /// Delete project (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.DeleteProjectAsync(id, userId);

        if (!result)
            return NotFound("Project not found or unauthorized");

        return Ok("Project deleted successfully");
    }
}
