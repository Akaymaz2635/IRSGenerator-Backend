using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.IRSProject;

namespace IRSGenerator.API.Controllers;

[Route("api/irs-projects")]
[ApiController]
public class IRSProjectsController : ControllerBase
{
    private readonly IIRSProjectRepository _repo;

    public IRSProjectsController(IIRSProjectRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IRSProjectReadDto>>> GetAll(
        [FromQuery] long? ownerId = null)
    {
        IEnumerable<IRSProject> items = ownerId.HasValue
            ? await _repo.GetByOwnerIdAsync(ownerId.Value)
            : await _repo.GetAllAsync();

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<IRSProjectReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<IRSProjectReadDto>> Create([FromBody] IRSProjectCreateDto dto)
    {
        var entity = new IRSProject
        {
            ProjectType  = dto.ProjectType,
            PartNumber   = dto.PartNumber,
            Operation    = dto.Operation,
            SerialNumber = dto.SerialNumber,
            OpSheetPath  = dto.OpSheetPath,
            OwnerId      = dto.OwnerId,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] IRSProjectCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        entity.ProjectType  = dto.ProjectType;
        entity.PartNumber   = dto.PartNumber;
        entity.Operation    = dto.Operation;
        entity.SerialNumber = dto.SerialNumber;
        entity.OpSheetPath  = dto.OpSheetPath;
        entity.OwnerId      = dto.OwnerId;
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

    private static IRSProjectReadDto ToDto(IRSProject e) => new()
    {
        Id           = e.Id,
        ProjectType  = e.ProjectType,
        PartNumber   = e.PartNumber,
        Operation    = e.Operation,
        SerialNumber = e.SerialNumber,
        OpSheetPath  = e.OpSheetPath,
        OwnerId      = e.OwnerId,
        CreatedAt    = e.CreatedAt,
        UpdatedAt    = e.UpdatedAt,
    };
}
