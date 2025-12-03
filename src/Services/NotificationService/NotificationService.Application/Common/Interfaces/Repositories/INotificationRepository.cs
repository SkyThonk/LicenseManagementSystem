using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Notification entity.
/// Each tenant has their own database, so no TenantId filtering is needed.
/// </summary>
public interface INotificationRepository
{
    void Add(Notification notification);
    void Update(Notification notification);
    Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetPendingNotificationsAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByRecipientAsync(string recipient, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated notifications with optional status filter - pagination happens at SQL level
    /// </summary>
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        string? status = null,
        CancellationToken cancellationToken = default);
}
