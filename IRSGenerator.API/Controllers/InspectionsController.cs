using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.API.Utils;
using MES.Domain.Dtos.Inspection;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InspectionsController : ControllerBase
{
    private readonly IInspectionService _service;
    private readonly IUnitOfWork _uow;
    private readonly WordOpSheetParser _parser;
    private readonly WordReportWriter _reportWriter;
    private readonly IWebHostEnvironment _env;

    public InspectionsController(
        IInspectionService service,
        IUnitOfWork uow,
        WordOpSheetParser parser,
        WordReportWriter reportWriter,
        IWebHostEnvironment env)
    {
        _service      = service;
        _uow          = uow;
        _parser       = parser;
        _reportWriter = reportWriter;
        _env          = env;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InspectionReadDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] long? project_id = null,
        [FromQuery] string? search = null)
        => Ok(await _service.GetAllAsync(status, project_id, search));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<InspectionReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<InspectionReadDto>> Create([FromBody] InspectionCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] InspectionUpdateDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpPost("{id:long}/complete")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Complete(long id)
    {
        var result = await _service.CompleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:long}/parse-opsheet")]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult> ParseOpSheet(long id, IFormFile file)
    {
        var inspection = await _uow.Inspections.GetByIdAsync(id);
        if (inspection is null) return NotFound();

        await using var stream = file.OpenReadStream();
        var characters = _parser.Parse(stream);
        foreach (var c in characters)
        {
            c.InspectionId = id;
            await _uow.Characters.AddAsync(c);
        }
        await _uow.CommitAsync();
        return Ok(new { message = $"{characters.Count} karakter içe aktarıldı." });
    }

    [HttpGet("{id:long}/report")]
    public async Task<ActionResult> GetReport(long id, [FromQuery] bool detail = false)
    {
        var inspection = await _uow.Inspections.GetByIdAsync(id, q => q
            .Include(i => i.VisualProject)
            .Include(i => i.IrsProject)
            .Include(i => i.InspectorUser)
            .Include(i => i.Defects).ThenInclude(d => d.DefectType)
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions)
            .Include(i => i.Defects).ThenInclude(d => d.ChildDefects)
            .Include(i => i.Photos).ThenInclude(p => p.PhotoDefects));
        if (inspection is null) return NotFound();

        var characters = _uow.Characters.Filter(c => c.InspectionId == id).ToList();
        var bytes = await _reportWriter.GenerateAsync(inspection, characters, _env.WebRootPath ?? _env.ContentRootPath, detail);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"report_{id}.docx");
    }
}
