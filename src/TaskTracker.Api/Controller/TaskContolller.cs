using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    // InMemory "database"
    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Learn HTTP basics", IsDone = false },
        new TaskItem { Id = 2, Title = "Open Swagger UI", IsDone = true }
    };

    [HttpGet]
    public ActionResult<List<TaskDto>> GetAll()
    {
        var result = _tasks
            .Select(t => new TaskDto { Id = t.Id, Title = t.Title, IsDone = t.IsDone })
            .ToList();

        return Ok(result); // 200
    }

    [HttpGet("{id:int}")]
    public ActionResult<TaskDto> GetById(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
            return NotFound(); // 404

        var dto = new TaskDto { Id = task.Id, Title = task.Title, IsDone = task.IsDone };
        return Ok(dto); // 200
    }
}
