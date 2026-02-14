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

    [HttpPost]
    public ActionResult<TaskDto> Create([FromBody] CreateTaskDto dto)
    {
        // dto already validated automatically because of [ApiController]
        var newId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;

        var task = new TaskItem
        {
            Id = newId,
            Title = dto.Title,
            IsDone = false
        };

        _tasks.Add(task);

        var result = new TaskDto { Id = task.Id, Title = task.Title, IsDone = task.IsDone };

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result
        ); // 201 + Location header
    }

    [HttpPut("{id:int}")]
    public ActionResult Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if ( task == null ) return NotFound();
        
        task.Title = dto.Title;
        task.IsDone = dto.IsDone;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if ( task == null ) return NotFound();

        _tasks.Remove(task);
        return NoContent();
    }
}
