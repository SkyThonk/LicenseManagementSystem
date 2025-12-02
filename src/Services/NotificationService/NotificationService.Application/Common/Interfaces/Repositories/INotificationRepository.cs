using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Notification entity
/// </summary>
public interface INotificationRepository
{
    void Add(Notification notification);
    void Update(Notification notification);
    Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(NotificationId id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetPendingNotificationsAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByRecipientAsync(string recipient, Guid tenantId, CancellationToken cancellationToken = default);
}
