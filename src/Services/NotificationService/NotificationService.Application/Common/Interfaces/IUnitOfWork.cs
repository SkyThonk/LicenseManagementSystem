namespace NotificationService.Application.Common.Interfaces;

/// <summary>
/// Unit of Work interface for NotificationService
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
