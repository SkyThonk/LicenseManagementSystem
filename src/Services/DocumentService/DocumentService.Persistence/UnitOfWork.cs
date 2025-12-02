using DocumentService.Application.Common.Interfaces;
using DocumentService.Persistence.Data;

namespace DocumentService.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
