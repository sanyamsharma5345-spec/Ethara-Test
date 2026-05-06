using TeamTaskManagerApi.DTOs;
using TeamTaskManagerApi.Models;
using TeamTaskManagerApi.Repositories;

namespace TeamTaskManagerApi.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IProjectRepository projectRepository,
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository,
        ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ProjectResponseDto?> CreateProjectAsync(Guid userId, CreateProjectDto dto)
    {
        try
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = userId
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            // Add creator as a member
            var projectMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = userId
            };

            await _projectMemberRepository.AddAsync(projectMember);
            await _projectMemberRepository.SaveChangesAsync();

            _logger.LogInformation("Project created: {ProjectId} by user: {UserId}", project.Id, userId);

            return new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedById = project.CreatedById,
                CreatedAt = project.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return null;
        }
    }

    public async Task<ProjectResponseDto?> GetProjectAsync(Guid projectId)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                _logger.LogWarning("Project not found: {ProjectId}", projectId);
                return null;
            }

            var members = project.Members
                .Select(pm => new UserResponseDto
                {
                    Id = pm.User!.Id,
                    Name = pm.User.Name,
                    Email = pm.User.Email,
                    Role = pm.User.Role
                })
                .ToList();

            return new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedById = project.CreatedById,
                CreatedAt = project.CreatedAt,
                Members = members
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project");
            return null;
        }
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(Guid userId)
    {
        try
        {
            var projects = await _projectRepository.GetByUserIdAsync(userId);
            return projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedById = p.CreatedById,
                CreatedAt = p.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user projects");
            return [];
        }
    }

    public async Task<bool> AddMemberAsync(Guid projectId, Guid adminId, AddProjectMemberDto dto)
    {
        try
        {
            // Verify admin is creator of project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.CreatedById != adminId)
            {
                _logger.LogWarning("Unauthorized: Admin {AdminId} cannot add member to project {ProjectId}", adminId, projectId);
                return false;
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", dto.UserId);
                return false;
            }

            // Check if already member
            var existingMember = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, dto.UserId);
            if (existingMember != null)
            {
                _logger.LogWarning("User already member of project");
                return false;
            }

            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = dto.UserId
            };

            await _projectMemberRepository.AddAsync(projectMember);
            await _projectMemberRepository.SaveChangesAsync();

            _logger.LogInformation("Member added to project: {ProjectId}, User: {UserId}", projectId, dto.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding project member");
            return false;
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetProjectMembersAsync(Guid projectId)
    {
        try
        {
            var members = await _projectMemberRepository.GetByProjectIdAsync(projectId);
            return members.Select(pm => new UserResponseDto
            {
                Id = pm.User!.Id,
                Name = pm.User.Name,
                Email = pm.User.Email,
                Role = pm.User.Role
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project members");
            return [];
        }
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid adminId)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.CreatedById != adminId)
            {
                _logger.LogWarning("Unauthorized: Cannot delete project");
                return false;
            }

            await _projectRepository.DeleteAsync(projectId);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Project deleted: {ProjectId}", projectId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project");
            return false;
        }
    }
}
