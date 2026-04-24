using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Services
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskTrackerDbContext _db;

        public ProjectRepository(TaskTrackerDbContext db) => _db = db;

        public Task AddAsync(Project project, CancellationToken ct) =>
            _db.Projects.AddAsync(project, ct).AsTask();

        public void Remove(Project project) => _db.Projects.Remove(project);

        public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
            _db.Projects.AnyAsync(project => project.Id == id, ct);

        public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken ct) =>
            await _db.Projects
                .AsNoTracking()
                .ToListAsync(ct);

        public Task<Project?> GetByIdAsync(int id, CancellationToken ct) =>
            _db.Projects.FirstOrDefaultAsync(project => project.Id == id, ct);

        public Task<Project?> GetByIdReadOnlyAsync(int id, CancellationToken ct) =>
            _db.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(project => project.Id == id, ct);

        public async Task<IReadOnlyCollection<TaskItem>> GetTasksByProjectIdAsync(int projectId, CancellationToken ct) =>
            await _db.Tasks
                .AsNoTracking()
                .Where(task => task.ProjectId == projectId)
                .OrderBy(task => task.Id)
                .ToListAsync(ct);

        public Task<bool> HasTasksAsync(int projectId, CancellationToken ct) =>
            _db.Tasks.AnyAsync(task => task.ProjectId == projectId, ct);

        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
