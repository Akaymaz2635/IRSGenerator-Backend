using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Project;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _repo;

    public ProjectsController(IProjectRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetAll([FromQuery] bool all = false)
    {
        var items = all
            ? await _repo.GetAllAsync()
            : await _repo.GetActiveAsync();

        var result = items.Select(p => new ProjectReadDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Customer = p.Customer,
            Active = p.Active,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProjectReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        return Ok(new ProjectReadDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Customer = entity.Customer,
            Active = entity.Active,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> Create([FromBody] ProjectCreateDto dto)
    {
        var entity = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            Customer = dto.Customer,
            Active = true
        };
        var created = await _repo.AddAsync(entity);
        var read = new ProjectReadDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Customer = created.Customer,
            Active = created.Active,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, read);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ProjectUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.Description is not null) entity.Description = dto.Description;
        if (dto.Customer is not null) entity.Customer = dto.Customer;
        if (dto.Active.HasValue) entity.Active = dto.Active.Value;

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        await _repo.DeleteAsync(entity);
        return NoContent();
    }
}
