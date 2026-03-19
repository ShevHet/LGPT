/* using Microsoft.Extensions.Options;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Options;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Services;

public class InMemoryTaskService : ITaskService
{
    private readonly TaskTrackerOptions _options;

    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Learn HTTP basics", Status = DomainTaskStatus.New, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        new TaskItem { Id = 2, Title = "Open Swagger UI", Status = DomainTaskStatus.Done, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
    };

    public InMemoryTaskService(IOptions<TaskTrackerOptions> options)
    {
        _options = options.Value;
    }

    public async Task<List<TaskDto>> GetAllAsync(CancellationToken ct)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        return _tasks.Select(ToDto).ToList();
    }

    public async Task<TaskDto> GetByIdAsync(int id, CancellationToken ct)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        ValidateId(id);

        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            throw new NotFoundException($"Task with id={id} was not found.");

        return ToDto(task);
    }

    public async Task<TaskDto> CreateAsync(string title, CancellationToken ct)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        ValidateTitle(title);

        if (_tasks.Count >= _options.MaxTasksLimit)
            throw new ValidationException($"Tasks limit reached: {_options.MaxTasksLimit}");

        if (!string.IsNullOrWhiteSpace(_options.DefaultTitlePrefix))
            title = $"{_options.DefaultTitlePrefix} {title}";

        var newId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Id = newId,
            Title = title,
            Status = DomainTaskStatus.New,
            CreatedAt = now,
            UpdatedAt = now
        };

        _tasks.Add(task);

        return ToDto(task);
    }

    public Task UpdateAsync(int id, string title, bool isDone, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        ValidateId(id);
        ValidateTitle(title);

        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            throw new NotFoundException($"Task with id={id} was not found.");

        task.Title = title;
        task.Status = isDone ? DomainTaskStatus.Done : DomainTaskStatus.InProgress;
        task.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ValidateId(id);

        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            throw new NotFoundException($"Task with id={id} was not found.");

        _tasks.Remove(task);
        return Task.CompletedTask;
    }

    private static TaskDto ToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        IsDone = task.Status == DomainTaskStatus.Done
    };

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Id must be a positive number.");
    }

    private static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title must not be empty.");

        if (title.Length < 3)
            throw new ValidationException("Title minimum length is 3.");
    }
    }
}
*/