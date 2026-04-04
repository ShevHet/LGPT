using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)=>
            _service = service;

        /// <summary>Returns a list projects.</summary>
        /// <param name="ct">Request cancellation token.</param>
        ///<response code = "200">Returns the project list.</response>
        [HttpGet]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetAll(CancellationToken ct)=>
            Ok(await _service.GetAllAsync(ct));

        /// <summary>Returns a project by id.</summary>
        /// <param name="id">Project id.</param>        
        /// <param name="ct">Request cancellation token.</param>
        ///<response code = "200">Return the project.</response>
        ///<response code = "400">Return 400 if the id is invalid.</response>
        ///<response code = "404">Return 404 if the project is not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<ActionResult<ProjectResponseDto>> GetById(int id, CancellationToken ct)=>
            Ok(await _service.GetByIdAsync(id, ct));

        /// <summary>Create a new project.</summary>
        /// <param name="dto">Project data.</param>
        /// <param name="ct">Request cancellation token</param>
        /// <response code = "201">Return the created project.</response>
        /// <response code = "400">Return 400 if the id is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] CreateProjectRequestDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Updates a project.</summary>
        /// <param name="id">Project id.</param>
        /// <param name="dto">Project data.</param>
        /// <param name="ct">Request cancellation token</param>
        /// <response code = "204">Project was updated.</response>
        /// <response code = "400">Returns 400 if the id is invalid.</response>
        /// <response code = "404">Returns 404 if the id not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProjectRequestDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        /// <summary>Deletes a project.</summary>
        /// <param name="id">Project id.</param>
        /// <param name="ct">Request cancellation token.</param>
        /// <response code="204">Project was deleted.</response>
        /// <response code="400">Returns 400 if id is invalid.</response>
        /// <response code="404">Returns 404 if the project is not found.</response>
        /// <response code="409">Returns 409 if the project has tasks.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);

            return NoContent();
        }

    }
}
