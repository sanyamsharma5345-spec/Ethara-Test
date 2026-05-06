using TeamTaskManagerApi.Models;

namespace TeamTaskManagerApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Project>> GetAllAsync();
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(Guid projectId);
    Task<ProjectMember?> GetByProjectAndUserAsync(Guid projectId, Guid userId);
    Task AddAsync(ProjectMember member);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskItem>> GetByAssignedToAsync(Guid userId);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}
