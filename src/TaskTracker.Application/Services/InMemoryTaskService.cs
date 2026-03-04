using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Services;

public class InMemoryTaskService : ITaskService
{
    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Learn HTTP basics", IsDone = false },
        new TaskItem { Id = 2, Title = "Open Swagger UI", IsDone = true }
    };

    public List<TaskDto> GetAll() =>
        _tasks.Select(t => new TaskDto { Id = t.Id, Title = t.Title, IsDone = t.IsDone }).ToList();

    public TaskDto? GetById(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return task == null ? null : new TaskDto { Id = task.Id, Title = task.Title, IsDone = task.IsDone };
    }

    public TaskDto Create(string title)
    {
        var newId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
        var task = new TaskItem { Id = newId, Title = title, IsDone = false };
        _tasks.Add(task);
        return new TaskDto { Id = newId, Title = title, IsDone = false };
    }

    public bool Update(int id, string title, bool isDone)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null) return false;

        task.Title = title;
        task.IsDone = isDone;
        return true;
    }

    public bool Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t=> t.Id == id);
        if(task == null) return false;

        _tasks.Remove(task);
        return true;
    }
}