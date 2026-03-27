using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    ValueTask<TEntity?> GetByIdAsync(long id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    Task<IEnumerable<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    Task<TEntity> AddAsync(TEntity entity);

    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);

    Task RemoveAsync(long entityId);

    void RemoveRange(IEnumerable<TEntity> entities);

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate,
        int? page = null, int? itemCount = null);
}
