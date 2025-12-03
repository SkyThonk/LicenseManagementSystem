using NotificationService.Domain.Templates;

namespace NotificationService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for NotificationTemplate entity.
/// Each tenant has their own database, so no TenantId filtering is needed.
/// </summary>
public interface INotificationTemplateRepository
{
    void Add(NotificationTemplate template);
    void Update(NotificationTemplate template);
    Task<NotificationTemplate?> GetByIdAsync(NotificationTemplateId id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByNameAsync(string templateName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string templateName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated templates with optional active filter - pagination happens at SQL level
    /// </summary>
    Task<(IReadOnlyList<NotificationTemplate> Items, int TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        bool activeOnly = false,
        CancellationToken cancellationToken = default);
}
