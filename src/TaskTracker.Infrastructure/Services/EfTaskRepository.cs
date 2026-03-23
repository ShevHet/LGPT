using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
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

        public async Task<IReadOnlyCollection<TaskItem>> GetAllAsync(CancellationToken ct) =>
            await _db.Tasks.AsNoTracking().ToListAsync(ct);

        public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Tasks.FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ProjectExistsAsync(int projectId, CancellationToken ct)=>
            _db.Projects.AnyAsync(x => x.Id == projectId,ct);

        public void Remove(TaskItem task) =>
            _db.Tasks.Remove(task);
        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
