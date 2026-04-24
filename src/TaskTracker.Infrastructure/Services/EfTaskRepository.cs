using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Persistence;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Infrastructure.Services
{
    public sealed class EfTaskRepository : ITaskRepository
    {
        private readonly TaskTrackerDbContext _db;

        public EfTaskRepository(TaskTrackerDbContext db) => _db = db;

        public Task AddAsync(TaskItem task, CancellationToken ct) =>
            _db.Tasks.AddAsync(task, ct).AsTask();

        public async Task<IReadOnlyCollection<TaskItem>> GetPagedAsync(
            int skip,
            int take,
            DomainTaskStatus? status,
            int? projectId,
            CancellationToken ct)
        {
            IQueryable<TaskItem> query = _db.Tasks
                .AsNoTracking()
                .Include(task => task.Project);

            if (status.HasValue)
                query = query.Where(task => task.Status == status.Value);

            if (projectId.HasValue)
                query = query.Where(task => task.ProjectId == projectId.Value);

            return await query
                .OrderBy(task => task.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Tasks.FirstOrDefaultAsync(task => task.Id == id, ct);

        public Task<TaskItem?> GetByIdReadOnlyAsync(int id, CancellationToken ct) =>
            _db.Tasks
                .AsNoTracking()
                .Include(task => task.Project)
                .FirstOrDefaultAsync(task => task.Id == id, ct);

        public void Remove(TaskItem task) => _db.Tasks.Remove(task);

        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
