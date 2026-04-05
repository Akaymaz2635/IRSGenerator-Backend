using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Project;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IVisualProjectRepository _repo;

    public ProjectsController(IVisualProjectRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetAll([FromQuery] bool all = false)
    {
        var items = all
            ? await _repo.GetAllAsync()
            : await _repo.GetActiveAsync();

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProjectReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProjectReadDto>> Create([FromBody] ProjectCreateDto dto)
    {
        var entity = new VisualProject
        {
            Name = dto.Name,
            Active = true
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] ProjectUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.Active.HasValue) entity.Active = dto.Active.Value;

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    // DELETE → soft delete (active = false)
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        entity.Active = false;
        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    private static ProjectReadDto ToReadDto(VisualProject p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Active = p.Active,
        CreatedAt = p.CreatedAt
    };
}
