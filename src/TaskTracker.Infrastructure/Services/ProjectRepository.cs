using Microsoft.EntityFrameworkCore;
using System.Threading;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Services
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskTrackerDbContext _db;

        public ProjectRepository(TaskTrackerDbContext db) =>_db = db;

        public Task AddAsync(Project project, CancellationToken ct) =>
            _db.Projects.AddAsync(project, ct).AsTask();

        public void Remove(Project project) =>
            _db.Projects.Remove(project);

        public async Task<bool> ExistsAsync(int id, CancellationToken ct)=>
            await _db.Projects.AnyAsync(p => p.Id == id, ct);

        public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken ct) =>
            await _db.Projects.AsNoTracking().ToListAsync(ct);
        

        public Task<Project?> GetByIdAsync(int id, CancellationToken ct)=>
            _db.Projects.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyCollection<TaskItem>> GetTasksByProjectIdAsync(int projectId, CancellationToken ct)=>
            await _db.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
            

        public async Task<bool> HasTasksAsync(int projectId, CancellationToken ct)=>
            await _db.Tasks.AnyAsync(t => t.ProjectId == projectId, ct);


        public Task SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);
    }
}
