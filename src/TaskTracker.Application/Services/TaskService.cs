using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Services
{
    public sealed class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IClock _clock;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IClock clock)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _clock = clock;
        }

        public async Task<TaskResponseDto> CreateAsync(CreateTaskRequestDto request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateCreateRequest(request);

            await EnsureProjectExistsAsync(request.ProjectId, ct);

            var now = _clock.UtcNow;
            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _taskRepository.AddAsync(task, ct);
            await _taskRepository.SaveChangesAsync(ct);

            return Map(task);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            ValidateId(id);

            var task = await _taskRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<IReadOnlyCollection<TaskResponseDto>> GetAllAsync(GetTaskRequestDto request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateGetAllRequest(request);

            var skip = CalculateSkip(request.Page, request.PageSize);
            var items = await _taskRepository.GetPagedAsync(skip, request.PageSize, request.Status, request.ProjectId, ct);

            return items.Select(Map).ToList();
        }

        public async Task<TaskResponseDto> GetByIdAsync(int id, CancellationToken ct)
        {
            ValidateId(id);

            var task = await _taskRepository.GetByIdReadOnlyAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            return Map(task);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequestDto request, CancellationToken ct)
        {
            ValidateId(id);
            ArgumentNullException.ThrowIfNull(request);
            ValidateUpdateRequest(request);

            var task = await _taskRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Task with id = {id} was not found");

            if (task.ProjectId != request.ProjectId)
                await EnsureProjectExistsAsync(request.ProjectId, ct);

            task.ProjectId = request.ProjectId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.UpdatedAt = _clock.UtcNow;

            await _taskRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task EnsureProjectExistsAsync(int projectId, CancellationToken ct)
        {
            if (!await _projectRepository.ExistsAsync(projectId, ct))
                throw new NotFoundException($"Project with id = {projectId} was not found");
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("Id must be a positive number");
        }

        private static void ValidateGetAllRequest(GetTaskRequestDto request)
        {
            if (request.ProjectId.HasValue && request.ProjectId.Value <= 0)
                throw new ValidationException("ProjectId must be a positive number.");

            if (request.Status.HasValue && !Enum.IsDefined(typeof(DomainTaskStatus), request.Status.Value))
                throw new ValidationException("Status must be one of: New, InProgress, Done.");
        }

        private static TaskResponseDto Map(TaskItem task) => new()
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        private static int CalculateSkip(int page, int pageSize)
        {
            var skip = ((long)page - 1) * pageSize;

            if (skip > int.MaxValue)
                throw new ValidationException("Request page is too large.");

            return (int)skip;
        }

        private static void ValidateCreateRequest(CreateTaskRequestDto request) =>
            ValidateTaskRequest(request.ProjectId, request.Title, request.Status);

        private static void ValidateUpdateRequest(UpdateTaskRequestDto request) =>
            ValidateTaskRequest(request.ProjectId, request.Title, request.Status);

        private static void ValidateTaskRequest(int projectId, string? title, DomainTaskStatus status)
        {
            if (projectId <= 0)
                throw new ValidationException("ProjectId must be a positive number.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Title is required.");

            if (title.Length > 200)
                throw new ValidationException("Title must be 200 characters or fewer.");

            if (!Enum.IsDefined(typeof(DomainTaskStatus), status))
                throw new ValidationException("Status must be one of: New, InProgress, Done");
        }
    }
}
