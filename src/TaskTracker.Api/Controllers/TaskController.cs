using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Dtos;
using TaskTracker.Api.Errors;
using TaskTracker.Application.Services;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>Returns a page of tasks</summary>
    /// <remarks>Filtering tasks by status and project id.</remarks>    
    /// <response code="200">Returns the task list.</response>
    /// <response code="400">Returns 400 if thw query is invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponseDto>>> GetAll(
        [FromQuery] GetTaskRequestDto request,
        CancellationToken ct)
    {       
        return Ok(await _service.GetAllAsync(request, ct)); // 200
    }

    /// <summary>Returns a task by id.</summary>
    /// <param name="id">Task id.</param>
    /// <param name="ct">Request cancellation  token</param>
    /// <response code="200">Returns the task.</response>
    /// <reposnse code="400">Returns 400 if id is invalid</reposnse>
    /// <response code="404">Returns 404 if the task is not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponseDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id, CancellationToken ct)
    {
        var task = await _service.GetByIdAsync(id, ct);
        
        return Ok(task); // 200
    }

    /// <summary>Create a new task.</summary>
    /// <param name="dto">Task data.</param>
    /// <param name="ct">Request cancellation token</param>
    /// <response code="201">Returns the created task.</response>
    /// <response code="400">Returns 400 if the request is invalid.</response>
    /// <response code="404">Returns 404 is the project is not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponseDto>> Create([FromBody] CreateTaskRequestDto dto,
        CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto,ct);

        return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
    }

    /// <summary>Update a task.</summary>
    /// <param name="id">Task id.</param>
    /// <param name="dto">Task data.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">Task was updated.</response>
    /// <response code="400">Returns 400 if the request is invalid.</response>
    /// <response code="404">Return 404 if the task is not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTaskRequestDto dto,
        CancellationToken ct)
    {
        await _service.UpdateAsync(id, dto, ct);
        
        return NoContent();
    }

    /// <summary>Delete a task.</summary>
    /// <param name="id">Task id.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <response code="204">Task was deleted.</response>
    /// <response code="400">Returns 400 if the id is invalid</response>
    /// <response code="404">Returns 404 if the task is not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);

        return NoContent();
    }
}
