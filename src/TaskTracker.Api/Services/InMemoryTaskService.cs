using System.Data.Common;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;

namespace TaskTracker.Api.Services;

public class InMemoryTaskService : ITaskService
{
    private readonly TaskTrackerOptions _options;

    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Learn HTTP basics", IsDone = false },
        new TaskItem { Id = 2, Title = "Open Swagger UI", IsDone = true }
    };

    public InMemoryTaskService(IOptions<TaskTrackerOptions> options)
    {
        _options = options.Value;
    }

    public Task<List<TaskDto>> GetAllAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var result = _tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            IsDone = t.IsDone
        }).ToList();

        return Task.FromResult(result);
    }


    public Task<TaskDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var task = _tasks.FirstOrDefault(t => t.Id == id);

        TaskDto? dto = task == null
            ? null
            : new TaskDto { Id = task.Id, Title = task.Title, IsDone = task.IsDone };

        return Task.FromResult(dto);
    }


    public Task<TaskDto> CreateAsync(string title, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (_tasks.Count >= _options.MaxTasksLimit)
            throw new ArgumentException($"Tasks limit reached: {_options.MaxTasksLimit}", nameof(title));

        if (!string.IsNullOrWhiteSpace(_options.DefaultTitlePrefix))
            title = $"{_options.DefaultTitlePrefix} {title}";

        var newId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
        
        var task = new TaskItem { Id = newId, Title = title, IsDone = false };
        
        _tasks.Add(task);

        var result = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            IsDone = task.IsDone
        };
        return Task.FromResult(result);
    }

    public Task<bool> UpdateAsync(int id, string title, bool isDone, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null) return Task.FromResult(false);

        task.Title = title;
        task.IsDone = isDone;
        return Task.FromResult(true);
    }

    public  Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var task = _tasks.FirstOrDefault(t=> t.Id == id);
        if(task == null) return Task.FromResult(false);

        _tasks.Remove(task);
        return Task.FromResult(true);
    }
}
