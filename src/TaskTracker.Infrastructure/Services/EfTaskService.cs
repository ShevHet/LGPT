using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Infrastructure.Services
{
    public sealed class EfTaskService : ITaskService
    {
        private readonly TaskTrackerDbContext _dbContext;

        public EfTaskService(TaskTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskResponseDto> CreateAsync(CreateTaskRequestDto request, CancellationToken ct)
        {
            var projectExists = await _dbContext.Projects
                .AnyAsync(project => project.Id == request.ProjectId, ct);

            if(!projectExists) 
                throw new InvalidOperationException($"Project with id {request.ProjectId} was not found.");

            var now = DateTime.UtcNow;

            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _dbContext.Tasks.AddAsync(task, ct);
            await _dbContext.SaveChangesAsync(ct);

            return new TaskResponseDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == id, ct);

            if(task == null)            
                return false;
            
            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync(ct);

            return true;
        }

        public async Task<IReadOnlyCollection<TaskResponseDto>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .Select(task => new TaskResponseDto
                {
                    Id = task.Id,
                    ProjectId = task.ProjectId,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<TaskResponseDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .Where(task => task.Id == id)
                .Select(task => new TaskResponseDto
                {
                    Id = task.Id,
                    ProjectId = task.ProjectId,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequestDto request, CancellationToken ct)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == id, ct);

            if(task == null)
                return false;

            var projectExists = await _dbContext.Projects
                .AnyAsync(project => project.Id == request.ProjectId);

            if(!projectExists)
                throw new InvalidOperationException($"Project with id {request.ProjectId} was not found.");

            task.ProjectId = request.ProjectId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return true;
        }
    }
}
