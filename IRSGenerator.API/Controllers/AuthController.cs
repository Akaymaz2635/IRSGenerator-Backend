using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Auth;
using System.Security.Cryptography;
using System.Text;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public AuthController(IUserRepository userRepo)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Sicil))
            return BadRequest(new { detail = "Sicil no boş olamaz." });

        var user = await _userRepo.GetByEmployeeIdAsync(dto.Sicil.Trim());

        if (user is null)
            return Unauthorized(new { detail = "Sicil numarası veya şifre hatalı." });

        if (!user.Active)
            return Unauthorized(new { detail = "Hesabınız pasife alınmış." });

        // Şifre henüz belirlenmemişse (PasswordHash null) ilk girişe izin ver
        if (user.PasswordHash is not null)
        {
            var hash = HashPassword(dto.Password ?? "");
            if (!string.Equals(user.PasswordHash, hash, StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { detail = "Sicil numarası veya şifre hatalı." });
        }

        return Ok(new LoginResponseDto
        {
            IsAdmin = user.Role == "admin",
            User = new LoginUserDto
            {
                Id = user.Id,
                Name = user.DisplayName,
                Role = user.Role
            }
        });
    }

    internal static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }
}
