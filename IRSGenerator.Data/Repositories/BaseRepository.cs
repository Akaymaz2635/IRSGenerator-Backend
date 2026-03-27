using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly IRSGeneratorDbContext Context;

    public BaseRepository(IRSGeneratorDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async ValueTask<TEntity?> GetByIdAsync(long id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (include != null) query = include(query);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (include != null) query = include(query);
        return await query.ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();
        var now = DateTime.UtcNow;
        foreach (var e in list) { e.CreatedAt = now; e.UpdatedAt = now; }
        await Context.Set<TEntity>().AddRangeAsync(list);
        await Context.SaveChangesAsync();
        return list;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveAsync(long entityId)
    {
        var entity = await Context.Set<TEntity>().FindAsync(entityId);
        if (entity != null)
        {
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
        }
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
        Context.SaveChanges();
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        if (include != null) query = include(query);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate,
        int? page = null, int? itemCount = null)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>().Where(predicate);
        if (page.HasValue && itemCount.HasValue)
            query = query.Skip((page.Value - 1) * itemCount.Value).Take(itemCount.Value);
        return query.ToList();
    }
}
