using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Services
{
    public sealed class TaskService : ITaskService
    {
        private readonly ITaskRepository _repo;

        public TaskService(ITaskRepository repo) => _repo = repo;
        public async Task<TaskResponseDto> CreateAsync(CreateTaskRequestDto request, CancellationToken ct)
        {
            if (!await _repo.ProjectExistsAsync(request.ProjectId, ct))
                throw new NotFoundException($"Project with id {request.ProjectId} was not found.");

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
            await _repo.AddAsync(task,ct);
            await _repo.SaveChangesAsync(ct);
            return Map(task);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            ValidateId(id);
            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            _repo.Remove(task);
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IReadOnlyCollection<TaskResponseDto>> GetAllAsync(CancellationToken ct)
        {
            var items = await _repo.GetAllAsync(ct);
            return items.Select(Map).ToList();
        }

        public async Task<TaskResponseDto> GetByIdAsync(int id, CancellationToken ct)
        {
            ValidateId(id);
            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");
            return Map(task);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequestDto request, CancellationToken ct)
        {
            ValidateId(id);

            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            task.ProjectId = request.ProjectId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync(ct);
            return true;
        }

        private static void ValidateId(int id)
        {
            if (id <= 0) throw new ValidationException("Id must be a positive number");
        }

        private static TaskResponseDto Map(TaskItem t) => new()
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
        };
    }
}
