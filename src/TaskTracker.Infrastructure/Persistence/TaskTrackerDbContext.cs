using Microsoft.EntityFramework;

namespace TaskTracker.Infrastructure.Persistence;

public sealed class TaskTrackerDbContext: DnContext
{
	public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
		: base(options)
	{

	}
}