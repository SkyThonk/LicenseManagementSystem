using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using TenantService.Persistence.Data;
using System.Linq.Expressions;


namespace TenantService.Persistence.Common.Abstractions;

internal abstract class Repository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : notnull
{
    protected readonly DataContext _dataContext;

    protected Repository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<TEntity>()
            .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _dataContext.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _dataContext.Set<TEntity>().Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dataContext.Set<TEntity>().UpdateRange(entities);
    }

    public virtual async Task<TEntity?> GetByAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dataContext.Set<TEntity>();

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<TEntity>()
            .ToListAsync(cancellationToken);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dataContext.Set<TEntity>().AddRange(entities);
    }

    public void Delete(TEntity entity)
    {
        _dataContext.Set<TEntity>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dataContext.Set<TEntity>().RemoveRange(entities);
    }
}

