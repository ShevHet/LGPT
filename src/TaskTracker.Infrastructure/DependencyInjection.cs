using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskTrackerDb");

        if (string.IsNullOrSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'TaskTrackerDb не найден");

        services.AddDbContext<TaskTrackerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}