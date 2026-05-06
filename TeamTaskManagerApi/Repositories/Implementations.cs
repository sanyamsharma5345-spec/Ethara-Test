using Microsoft.EntityFrameworkCore;
using TeamTaskManagerApi.Data;
using TeamTaskManagerApi.Models;

namespace TeamTaskManagerApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.Members)
            .ThenInclude(pm => pm.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Projects
            .Where(p => p.CreatedById == userId || p.Members.Any(pm => pm.UserId == userId))
            .Include(p => p.Members)
            .ThenInclude(pm => pm.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects.Include(p => p.Members).ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await GetByIdAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectMember?> GetByIdAsync(Guid id)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.User)
            .Include(pm => pm.Project)
            .FirstOrDefaultAsync(pm => pm.Id == id);
    }

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetByProjectAndUserAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
    }

    public async Task AddAsync(ProjectMember member)
    {
        await _context.ProjectMembers.AddAsync(member);
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _context.ProjectMembers.FindAsync(id);
        if (member != null)
        {
            _context.ProjectMembers.Remove(member);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.TaskItems
            .Include(t => t.AssignedToUser)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.TaskItems
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.AssignedToUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByAssignedToAsync(Guid userId)
    {
        return await _context.TaskItems
            .Where(t => t.AssignedToId == userId)
            .Include(t => t.Project)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
    {
        return await _context.TaskItems
            .Where(t => t.Status != "Completed" && t.DueDate < DateTime.UtcNow)
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems
            .Include(t => t.AssignedToUser)
            .Include(t => t.Project)
            .ToListAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.TaskItems.Remove(task);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
