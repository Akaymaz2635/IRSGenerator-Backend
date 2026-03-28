using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.User;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll([FromQuery] bool all = true)
    {
        var items = await _repo.GetAllAsync();
        if (!all)
            items = items.Where(u => u.Active);

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            return BadRequest(new { detail = "Sicil no boş olamaz." });
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { detail = "Şifre boş olamaz." });

        // Sicil çakışması
        var existing = await _repo.GetByEmployeeIdAsync(dto.EmployeeId.Trim());
        if (existing is not null)
            return Conflict(new { detail = "Bu sicil numarası zaten kayıtlı." });

        var entity = new User
        {
            EmployeeId = dto.EmployeeId.Trim(),
            DisplayName = dto.Name,
            FirstName = dto.Name,
            LastName = "",
            WindowsAccount = "",
            Role = dto.Role,
            Active = true,
            PasswordHash = AuthController.HashPassword(dto.Password)
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UserUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.Name is not null) { entity.DisplayName = dto.Name; entity.FirstName = dto.Name; }
        if (dto.Role is not null) entity.Role = dto.Role;
        if (dto.Active.HasValue) entity.Active = dto.Active.Value;
        if (!string.IsNullOrEmpty(dto.Password))
            entity.PasswordHash = AuthController.HashPassword(dto.Password);

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    // DELETE → soft delete (active = false)
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        entity.Active = false;
        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    private static UserReadDto ToReadDto(User u) => new()
    {
        Id = u.Id,
        EmployeeId = u.EmployeeId,
        Name = u.DisplayName,
        Role = u.Role,
        Active = u.Active,
        CreatedAt = u.CreatedAt
    };
}
