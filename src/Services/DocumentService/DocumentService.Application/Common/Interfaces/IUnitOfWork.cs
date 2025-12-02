namespace DocumentService.Application.Common.Interfaces;

/// <summary>
/// Unit of Work interface for DocumentService
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
