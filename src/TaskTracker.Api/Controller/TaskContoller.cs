using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;
using TaskTracker.Api.Services;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>Get all tasks</summary>
    /// <remarks>Returns all tasks currently stored in memory.</remarks>
    /// <returns>List of tasks</returns>
    /// <response code="200">Tasks were returned successfully</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<List<TaskDto>> GetAll()
    {       
        return Ok(_service.GetAll()); // 200
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
        var task = _service.GetById(id);
        if (task is null)
        {
            return NotFound(new ApiErrorResponse(
                TraceId: HttpContext.TraceIdentifier,
                Message: $"Task with id '{id}' not found",
                Errors: null
            ));
        }
        return Ok(task); // 200
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
        var created = _service.Create(dto.Title);

        return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
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
        var updated = _service.Update(id, dto.Title, dto.IsDone);
        if(!updated)
        {
            return NotFound(new ApiErrorResponse(
                TraceId: HttpContext.TraceIdentifier,
                Message: $"Task with id '{id}' not found",
                Errors: null
            ));
        }

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
        var deleted = _service.Delete(id);
        if (!deleted)
        {
            return NotFound(new ApiErrorResponse(
                TraceId: HttpContext.TraceIdentifier,
                Message: $"Task with id '{id}' not found",
                Errors: null
            ));
        }

        return NoContent();
    }
}
