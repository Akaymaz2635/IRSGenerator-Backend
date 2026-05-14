using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Auth;
using System.Security.Claims;

namespace MES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Sicil))
            return BadRequest(new { detail = "Sicil no boş olamaz." });

        var result = await _authService.ValidateCredentialsAsync(dto);
        if (result is null)
            return Unauthorized(new { detail = "Sicil numarası veya şifre hatalı ya da hesap pasif." });

        var claims = new List<Claim>
        {
            new("userId", result.User!.Id.ToString()),
            new("role",   result.User.Role ?? "inspector"),
            new(ClaimTypes.Name, result.User.Name ?? dto.Sicil),
        };
        var identity  = new ClaimsIdentity(claims, "AppCookie");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("AppCookie", principal);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AppCookie");
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst("userId")?.Value;
        var role   = User.FindFirst("role")?.Value;
        var name   = User.FindFirst(ClaimTypes.Name)?.Value;
        return Ok(new { user_id = userId, role, name });
    }
}
