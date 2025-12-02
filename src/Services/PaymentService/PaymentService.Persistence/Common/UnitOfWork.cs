using PaymentService.Application.Common.Interfaces;
using PaymentService.Persistence.Data;

namespace PaymentService.Persistence.Common;

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
