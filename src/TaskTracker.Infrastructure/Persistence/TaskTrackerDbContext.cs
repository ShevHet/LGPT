using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Models;

namespace TaskTracker.Infrastructure.Persistence;

public sealed class TaskTrackerDbContext : DbContext
{
	public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
		: base(options)
	{

	}

	public DbSet<Project> Projects => Set<Project>();
	public DbSet<TaskItem> Tasks => Set<TaskItem>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskTrackerDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
    }
}
