using TenantService.Persistence.Data;
using TenantService.Application.Common.Interfaces;

namespace TenantService.Persistence.Common;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;
    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.SaveChangesAsync(cancellationToken);
    }
}

