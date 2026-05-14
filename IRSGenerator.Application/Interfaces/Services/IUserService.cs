using MES.Domain.Dtos.User;

namespace MES.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllAsync();
    Task<UserReadDto?> GetByIdAsync(long id);
    Task<UserReadDto> CreateAsync(UserCreateDto dto);
    Task UpdateAsync(long id, UserUpdateDto dto);
    Task DeleteAsync(long id);
}
