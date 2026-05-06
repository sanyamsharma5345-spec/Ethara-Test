using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Services;

namespace TeamTaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
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
    /// Get overall dashboard stats
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get stats for a specific project
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectStats(Guid projectId)
    {
        var result = await _dashboardService.GetProjectStatsAsync(projectId);
        return Ok(result);
    }

    /// <summary>
    /// Get stats for current user
    /// </summary>
    [HttpGet("user")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserStats()
    {
        var userId = GetCurrentUserId();
        var result = await _dashboardService.GetUserStatsAsync(userId);
        return Ok(result);
    }
}
