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

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetAll(CancellationToken ct)=>
            Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponseDto>> GetById(int id, CancellationToken ct)=>
            Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] CreateProjectRequestDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProjectRequestDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);

            return NoContent();
        }

    }
}
