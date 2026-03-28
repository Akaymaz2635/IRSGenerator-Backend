using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetByIdAsync(long id, Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null);
    Task<IEnumerable<User>> GetAllAsync(Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null);
    Task<User?> GetByEmployeeIdAsync(string employeeId);
    Task<User> AddAsync(User entity);
    Task UpdateAsync(User entity);
    Task DeleteAsync(User entity);
}
