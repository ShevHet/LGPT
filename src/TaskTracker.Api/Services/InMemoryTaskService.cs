using System.Data.Common;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;
using TaskTracker.Api.Options;
using TaskTracker.Api.Exceptions;

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


    public Task<TaskDto> GetByIdAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        ValidateId(id);
       
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if(task is null) 
            throw new NotFoundException($"Task with id={id} was not found.");
        var dto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            IsDone = task.IsDone
        };

        return Task.FromResult(dto);
    }


    public Task<TaskDto> CreateAsync(string title, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        ValidateTitle(title);

        if (_tasks.Count >= _options.MaxTasksLimit)
            throw new ValidationException($"Tasks limit reached: {_options.MaxTasksLimit}");

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

    public Task UpdateAsync(int id, string title, bool isDone, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        ValidateId(id);
        ValidateTitle(title);

        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null) 
             throw new NotFoundException($"Task with id={id} was not found.");

        task.Title = title;
        task.IsDone = isDone;

        return Task.CompletedTask;
    }

    public  Task DeleteAsync(int id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        ValidateId(id);

        var task = _tasks.FirstOrDefault(t=> t.Id == id);
        if(task == null) 
            throw new NotFoundException($"Task with id={id} was not found.");

        _tasks.Remove(task);
        return Task.CompletedTask;
    }

    private static void ValidateId(int id)
    {
        if(id <= 0)
            throw new ValidationException("Id must be a positive number.");
    }

    private static void ValidateTitle(string? title)
    {
        if(string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title must not be empty.");
        
        if (title.Length >= 3)
            throw new ValidationException("Минимальная длина Title - 3.");
    }
}
