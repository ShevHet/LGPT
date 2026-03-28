using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Services
{
    public sealed class EfTaskRepository : ITaskRepository
    {
        private readonly TaskTrackerDbContext _db;

        public EfTaskRepository(TaskTrackerDbContext db)=>_db = db;

        public Task AddAsync(TaskItem task, CancellationToken ct) =>
            _db.Tasks.AddAsync(task, ct).AsTask();

        public async Task<IReadOnlyCollection<TaskItem>> GetPagedAsync(int skip, int take, CancellationToken ct) =>
            await _db.Tasks
            .AsNoTracking()
            .OrderBy(x=>x.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);            

        public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<bool> ProjectExistsAsync(int projectId, CancellationToken ct)=>
            _db.Projects.AnyAsync(x => x.Id == projectId,ct);

        public void Remove(TaskItem task) =>
            _db.Tasks.Remove(task);
        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
