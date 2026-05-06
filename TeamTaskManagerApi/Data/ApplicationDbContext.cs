using Microsoft.EntityFrameworkCore;
using TeamTaskManagerApi.Models;

namespace TeamTaskManagerApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> Project (one-to-many)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> ProjectMember (one-to-many)
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project -> ProjectMember (one-to-many)
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> TaskItem (one-to-many)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        // Project -> TaskItem (one-to-many)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint on Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Seed initial data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var memberId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Seed Admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                Name = "Admin User",
                Email = "admin@teamtaskmanager.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed Member user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = memberId,
                Name = "Member User",
                Email = "member@teamtaskmanager.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
                Role = "Member",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
