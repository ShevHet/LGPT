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

    /// <summary>Get all tasks</summary>
    /// <remarks>Returns all tasks currently stored in memory.</remarks>
    /// <returns>List of tasks</returns>
    /// <response code="200">Tasks were returned successfully</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<List<TaskDto>> GetAll()
    {
        var result = _tasks
            .Select(t => new TaskDto { Id = t.Id, Title = t.Title, IsDone = t.IsDone })
            .ToList();

        return Ok(result); // 200
    }

    //// <summary>Get a task by id</summary>
    /// <param name="id">Task id</param>
    /// <returns>Task</returns>
    /// <response code="200">Task was found and returned</response>
    /// <response code="404">Task with the given id was not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskDto> GetById(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
            return NotFound(); // 404

        var dto = new TaskDto { Id = task.Id, Title = task.Title, IsDone = task.IsDone };
        return Ok(dto); // 200
    }

    /// <summary>Create a new task</summary>
    /// <param name="dto">Task data</param>
    /// <returns>Created task</returns>
    /// <response code="201">Task was created successfully</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>Update an existing task (full update)</summary>
    /// <param name="id">Task id</param>
    /// <param name="dto">Updated task data</param>
    /// <response code="204">Task was updated successfully</response>
    /// <response code="400">Validation failed</response>
    /// <response code="404">Task with the given id was not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if ( task == null ) return NotFound();
        
        task.Title = dto.Title;
        task.IsDone = dto.IsDone;

        return NoContent();
    }

    /// <summary>Delete a task</summary>
    /// <param name="id">Task id</param>
    /// <response code="204">Task was deleted successfully</response>
    /// <response code="404">Task with the given id was not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if ( task == null ) return NotFound();

        _tasks.Remove(task);
        return NoContent();
    }
}
