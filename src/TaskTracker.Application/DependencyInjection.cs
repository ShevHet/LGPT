using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection.Metadata;
using TaskTracker.Application.Services;

namespace TaskTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }
    }
}
