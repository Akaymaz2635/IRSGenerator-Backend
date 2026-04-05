using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.DefectField;

namespace IRSGenerator.API.Controllers;

[Route("api/defect-fields")]
[ApiController]
[Authorize]
public class DefectFieldsController : ControllerBase
{
    private readonly IDefectFieldRepository _repo;

    public DefectFieldsController(IDefectFieldRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectFieldReadDto>>> GetAll(
        [FromQuery] long? defect_type_id = null)
    {
        IEnumerable<DefectField> items;
        if (defect_type_id.HasValue)
            items = await _repo.GetByDefectTypeAsync(defect_type_id.Value);
        else
            items = await _repo.GetAllAsync();

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectFieldReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DefectFieldReadDto>> Create([FromBody] DefectFieldCreateDto dto)
    {
        var entity = new DefectField
        {
            DefectTypeId = dto.DefectTypeId,
            FieldName = dto.FieldName,
            Label = dto.Label,
            FieldType = dto.FieldType,
            Required = dto.Required,
            Unit = dto.Unit,
            MinValue = dto.MinValue,
            MaxValue = dto.MaxValue,
            SortOrder = dto.SortOrder
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectFieldUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.Label is not null) entity.Label = dto.Label;
        if (dto.FieldType is not null) entity.FieldType = dto.FieldType;
        if (dto.Required.HasValue) entity.Required = dto.Required.Value;
        if (dto.Unit is not null) entity.Unit = dto.Unit;
        if (dto.MinValue.HasValue) entity.MinValue = dto.MinValue.Value;
        if (dto.MaxValue.HasValue) entity.MaxValue = dto.MaxValue.Value;
        if (dto.SortOrder.HasValue) entity.SortOrder = dto.SortOrder.Value;

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static DefectFieldReadDto ToReadDto(DefectField f) => new()
    {
        Id = f.Id,
        DefectTypeId = f.DefectTypeId,
        FieldName = f.FieldName,
        Label = f.Label,
        FieldType = f.FieldType,
        Required = f.Required,
        Unit = f.Unit,
        MinValue = f.MinValue,
        MaxValue = f.MaxValue,
        SortOrder = f.SortOrder,
        CreatedAt = f.CreatedAt
    };
}
