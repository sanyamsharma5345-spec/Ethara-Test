using TeamTaskManagerApi.Models;
using BCrypt.Net;

namespace TeamTaskManagerApi.Data;

public static class DatabaseSeeder
{
    public static void SeedDemoData(ApplicationDbContext context)
    {
        // Don't seed if data already exists
        if (context.Projects.Any() || context.TaskItems.Any())
        {
            return;
        }

        // Get existing users (created during initial migration)
        var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@teamtaskmanager.com");
        var memberUser = context.Users.FirstOrDefault(u => u.Email == "member@teamtaskmanager.com");

        if (adminUser == null || memberUser == null)
        {
            return;
        }

        // Create demo projects
        var project1 = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Website Redesign",
            Description = "Complete redesign of the company website with modern UI/UX",
            CreatedById = adminUser.Id,
            CreatedByUser = adminUser,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        var project2 = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Mobile App Development",
            Description = "Build native mobile app for iOS and Android platforms",
            CreatedById = adminUser.Id,
            CreatedByUser = adminUser,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        };

        var project3 = new Project
        {
            Id = Guid.NewGuid(),
            Name = "API Integration",
            Description = "Integrate third-party payment gateway and analytics APIs",
            CreatedById = adminUser.Id,
            CreatedByUser = adminUser,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        context.Projects.AddRange(project1, project2, project3);
        context.SaveChanges();

        // Add member to projects
        var projectMember1 = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = project1.Id,
            UserId = memberUser.Id,
            AddedAt = DateTime.UtcNow.AddDays(-19)
        };

        var projectMember2 = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = project2.Id,
            UserId = memberUser.Id,
            AddedAt = DateTime.UtcNow.AddDays(-14)
        };

        var projectMember3 = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = project3.Id,
            UserId = memberUser.Id,
            AddedAt = DateTime.UtcNow.AddDays(-9)
        };

        context.ProjectMembers.AddRange(projectMember1, projectMember2, projectMember3);
        context.SaveChanges();

        // Create demo tasks for Project 1: Website Redesign
        var task1 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Create wireframes",
            Description = "Design wireframes for all pages",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(-5),
            AssignedToId = memberUser.Id,
            ProjectId = project1.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-18),
            CompletedAt = DateTime.UtcNow.AddDays(-4)
        };

        var task2 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Design mockups",
            Description = "Create high-fidelity mockups in Figma",
            Status = "InProgress",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedToId = memberUser.Id,
            ProjectId = project1.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-12)
        };

        var task3 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Frontend development",
            Description = "Develop frontend using React and Tailwind CSS",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(10),
            AssignedToId = memberUser.Id,
            ProjectId = project1.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var task4 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Backend API integration",
            Description = "Connect frontend to backend APIs",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(-2), // Overdue
            AssignedToId = memberUser.Id,
            ProjectId = project1.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        };

        // Create demo tasks for Project 2: Mobile App Development
        var task5 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Setup development environment",
            Description = "Install necessary tools and SDKs",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(-10),
            AssignedToId = memberUser.Id,
            ProjectId = project2.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-13),
            CompletedAt = DateTime.UtcNow.AddDays(-10)
        };

        var task6 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Design app UI",
            Description = "Create user interface designs",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(-3),
            AssignedToId = memberUser.Id,
            ProjectId = project2.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-11),
            CompletedAt = DateTime.UtcNow.AddDays(-3)
        };

        var task7 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Implement authentication",
            Description = "Add login and registration features",
            Status = "InProgress",
            DueDate = DateTime.UtcNow.AddDays(5),
            AssignedToId = memberUser.Id,
            ProjectId = project2.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        var task8 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Build database layer",
            Description = "Create SQLite database and models",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(12),
            AssignedToId = memberUser.Id,
            ProjectId = project2.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        // Create demo tasks for Project 3: API Integration
        var task9 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Research payment gateways",
            Description = "Compare different payment gateway options",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(-7),
            AssignedToId = memberUser.Id,
            ProjectId = project3.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            CompletedAt = DateTime.UtcNow.AddDays(-7)
        };

        var task10 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Integrate Stripe",
            Description = "Integrate Stripe payment gateway",
            Status = "InProgress",
            DueDate = DateTime.UtcNow.AddDays(2),
            AssignedToId = memberUser.Id,
            ProjectId = project3.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var task11 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Setup Google Analytics",
            Description = "Configure analytics tracking",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(8),
            AssignedToId = memberUser.Id,
            ProjectId = project3.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var task12 = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Testing and bug fixes",
            Description = "Test all integrations and fix bugs",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(-1), // Overdue
            AssignedToId = memberUser.Id,
            ProjectId = project3.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        context.TaskItems.AddRange(
            task1, task2, task3, task4,
            task5, task6, task7, task8,
            task9, task10, task11, task12
        );

        context.SaveChanges();
    }
}
