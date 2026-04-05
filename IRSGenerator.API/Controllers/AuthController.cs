using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Auth;
using System.Security.Claims;
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

        // HttpOnly cookie oluştur — frontend localStorage yerine bu cookie kullanılır
        var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("role",   user.Role),
            new(ClaimTypes.Name, user.DisplayName ?? user.EmployeeId),
        };
        var identity  = new ClaimsIdentity(claims, "AppCookie");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("AppCookie", principal);

        return Ok(new LoginResponseDto
        {
            IsAdmin = user.Role == "admin",
            User = new LoginUserDto
            {
                Id   = user.Id,
                Name = user.DisplayName,
                Role = user.Role
            }
        });
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AppCookie");
        return NoContent();
    }

    // GET /api/auth/me  — oturum bilgisini döner
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst("userId")?.Value;
        var role   = User.FindFirst("role")?.Value;
        var name   = User.FindFirst(ClaimTypes.Name)?.Value;
        return Ok(new { user_id = userId, role, name });
    }

    internal static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }
}
