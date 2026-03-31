using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Services;

public interface ITaskRepository
{
    Task<IReadOnlyCollection<TaskItem>> GetPagedAsync(int skip, int take,
        DomainTaskStatus? staus, int? projectId, CancellationToken ct);
    Task<TaskItem?> GetByIdReadOnlyAsync(int id, CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ProjectExistsAsync(int projectId, CancellationToken ct);
    Task AddAsync(TaskItem task, CancellationToken ct);
    void Remove(TaskItem task);
    Task SaveChangesAsync(CancellationToken ct);
}
