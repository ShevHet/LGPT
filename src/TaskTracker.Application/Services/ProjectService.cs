using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;

        public ProjectService(IProjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProjectResponseDto> CreateAsync(CreateProjectRequestDto request, CancellationToken ct)
        {
            var project = new Project
            {
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(project, ct);
            await _repo.SaveChangesAsync(ct);

            return Map(project);
        }                                       

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            ValidateId(id);

            var project = await _repo.GetByIdAsync(id, ct);

            if(project == null)            
                throw new NotFoundException($"Project with id = {id} was not found");
            
            var hasTasks = await _repo.HasTasksAsync(id, ct);

            if (hasTasks)
                throw new ConflictException("Cannot delete a project that has tasks.");

            _repo.Remove(project);
            await _repo.SaveChangesAsync(ct);

            return true;
        }

        public async Task<IReadOnlyCollection<ProjectResponseDto>> GetAllAsync(CancellationToken ct)
        {
             var project = await _repo.GetAllAsync(ct);

            return project.Select(Map).ToList();
        }

        public async Task<ProjectResponseDto> GetByIdAsync(int id, CancellationToken ct)
        {
            ValidateId(id);
            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Project with id = {id} was not found");

            return Map(task);
        }

        public async Task<IReadOnlyCollection<TaskResponseDto>> GetTasksByProjectIdAsync(int projectId, CancellationToken ct)
        {
            ValidateId(projectId);

            var projectExists = await _repo.ExistsAsync(projectId, ct);

            if (!projectExists)
                throw new NotFoundException($"Project with id = {projectId} was not found");

            var tasks = await _repo.GetTasksByProjectIdAsync(projectId, ct);                

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt

            }).ToList();
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectRequestDto request, CancellationToken ct)
        {
            ValidateId(id);
            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Project with id = {id} was not found");

            task.Name = request.Name;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        private static void ValidateId(int id)
        {
            if (id <= 0) throw new ValidationException("Id must be a positive number");
        }

        private static ProjectResponseDto Map(Project pr) => new()
        {
            Id = pr.Id,
            Name = pr.Name,
            CreatedAt = pr.CreatedAt,
        };
    }
}
