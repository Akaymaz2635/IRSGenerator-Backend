using MES.Domain.Dtos.Auth;

namespace MES.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> ValidateCredentialsAsync(LoginRequestDto dto);
}
