using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Domain.Templates;
using NotificationService.Persistence.Common.Abstractions;
using NotificationService.Persistence.Data;

namespace NotificationService.Persistence.Repositories;

/// <summary>
/// Repository for NotificationTemplate entity persistence operations
/// </summary>
internal sealed class NotificationTemplateRepository : Repository<NotificationTemplate, NotificationTemplateId>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(DataContext dataContext)
        : base(dataContext)
    {
    }

    public async Task<NotificationTemplate?> GetByIdAsync(NotificationTemplateId id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetByNameAsync(string templateName, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .FirstOrDefaultAsync(t => t.TemplateName.ToLower() == templateName.ToLower() && t.TenantId == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .Where(t => t.TenantId == tenantId)
            .OrderBy(t => t.TemplateName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .Where(t => t.TenantId == tenantId && t.IsActive)
            .OrderBy(t => t.TemplateName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string templateName, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .AnyAsync(t => t.TemplateName.ToLower() == templateName.ToLower() && t.TenantId == tenantId, cancellationToken);
    }
}
