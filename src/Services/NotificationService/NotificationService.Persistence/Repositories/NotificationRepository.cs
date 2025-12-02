using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Domain.Common.Enums;
using NotificationService.Domain.Notifications;
using NotificationService.Persistence.Common.Abstractions;
using NotificationService.Persistence.Data;

namespace NotificationService.Persistence.Repositories;

/// <summary>
/// Repository for Notification entity persistence operations
/// </summary>
internal sealed class NotificationRepository : Repository<Notification, NotificationId>, INotificationRepository
{
    public NotificationRepository(DataContext dataContext)
        : base(dataContext)
    {
    }

    public async Task<Notification?> GetByIdAsync(NotificationId id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Notification>()
            .FirstOrDefaultAsync(n => n.Id == id && n.TenantId == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Notification>()
            .Where(n => n.TenantId == tenantId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetPendingNotificationsAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Notification>()
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderBy(n => n.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByRecipientAsync(string recipient, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Notification>()
            .Where(n => n.Recipient == recipient && n.TenantId == tenantId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
