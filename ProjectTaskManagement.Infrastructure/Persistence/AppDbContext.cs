using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Infrastructure.Identity;

namespace ProjectTaskManagement.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }

    public DbSet<TaskItem> Tasks { get; set; }
}