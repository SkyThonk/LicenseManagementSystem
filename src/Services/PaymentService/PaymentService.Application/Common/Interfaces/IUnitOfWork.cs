namespace PaymentService.Application.Common.Interfaces;

/// <summary>
/// Unit of Work interface for PaymentService
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
