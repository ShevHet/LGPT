using System.Diagnostics;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;


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
                CreatedAt = DateTime.UtcNow,
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

        public async Task<IReadOnlyCollection<TaskResponseDto>> GetAllAsync(GetTaskRequestDto request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateGetAllRequest(request);

            var skip = CalculateSkip(request.Page, request.PageSize);
            var items = await _repo.GetPagedAsync(skip, request.PageSize,request.Status, request.ProjectId, ct);
            
            return items.Select(Map).ToList();
        }

        public async Task<TaskResponseDto> GetByIdAsync(int id, CancellationToken ct)
        {
            ValidateId(id);
            var task = await _repo.GetByIdReadOnlyAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");
            return Map(task);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequestDto request, CancellationToken ct)
        {
            ValidateId(id);

            var task = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            if (!await _repo.ProjectExistsAsync(request.ProjectId, ct))
                throw new NotFoundException($"Project with id = {id} was not found");

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

        private static void ValidateGetAllRequest(GetTaskRequestDto request)
        {
            if (request.ProjectId.HasValue && request.ProjectId.Value <= 0)
                throw new ValidationException("ProjectId must be a positive number.");

            if (request.Status.HasValue && !Enum.IsDefined(typeof(DomainTaskStatus), request.Status.Value))
                throw new ValidationException("Status must be one of: New, InProgress, Done.");
        }

        private static TaskResponseDto Map(TaskItem t) => new()
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            ProjectName = t.Project.Name,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
        };

        private static int CalculateSkip(int page, int pageSize)
        {
            var skip = ((long)page - 1) * pageSize;

            if (skip > int.MaxValue)
                throw new ValidationException("Request page is too large.");

            return (int)skip;
        }
    }
}
