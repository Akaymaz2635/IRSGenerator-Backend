using System.Security.Cryptography;
using System.Text;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Auth;

namespace MES.API.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;

    public AuthService(IUnitOfWork uow) => _uow = uow;

    public async Task<LoginResponseDto?> ValidateCredentialsAsync(LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Sicil)) return null;

        var user = await _uow.Users.FindAsync(u => u.EmployeeId == dto.Sicil.Trim());
        if (user is null || !user.Active) return null;

        if (user.PasswordHash is not null)
        {
            var hash = HashPassword(dto.Password ?? "");
            if (!string.Equals(user.PasswordHash, hash, StringComparison.OrdinalIgnoreCase))
                return null;
        }

        return new LoginResponseDto
        {
            IsAdmin = user.Role == "admin",
            User = new LoginUserDto
            {
                Id   = user.Id,
                Name = user.DisplayName,
                Role = user.Role
            }
        };
    }

    internal static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }
}
