using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Services
{
    public interface IProjectRepository
    {
        Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken ct);
        Task<Project?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> ExistsAsync(int id, CancellationToken ct);
        Task AddAsync(Project project, CancellationToken ct);
        void DeleteAsync(Project project);
        Task<bool> HasTasksAsync(int projectId, CancellationToken ct);
        Task<IReadOnlyCollection<TaskItem>> GetTasksByProjectIdAsync(int projectId, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
