using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.User;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllAsync()
    {
        var items = await _uow.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserReadDto>>(items);
    }

    public async Task<UserReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Users.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<UserReadDto>(entity);
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
    {
        var entity = _mapper.Map<User>(dto);
        var created = await _uow.Users.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<UserReadDto>(created);
    }

    public async Task UpdateAsync(long id, UserUpdateDto dto)
    {
        var entity = await _uow.Users.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<User>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.Users.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<User>(id);
        await _uow.Users.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
