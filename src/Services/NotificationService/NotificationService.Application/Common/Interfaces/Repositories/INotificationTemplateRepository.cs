using NotificationService.Domain.Templates;

namespace NotificationService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for NotificationTemplate entity
/// </summary>
public interface INotificationTemplateRepository
{
    void Add(NotificationTemplate template);
    void Update(NotificationTemplate template);
    Task<NotificationTemplate?> GetByIdAsync(NotificationTemplateId id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByIdAsync(NotificationTemplateId id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByNameAsync(string templateName, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string templateName, Guid tenantId, CancellationToken cancellationToken = default);
}
