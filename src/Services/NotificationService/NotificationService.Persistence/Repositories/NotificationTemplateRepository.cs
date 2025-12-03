using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Domain.Templates;
using NotificationService.Persistence.Common.Abstractions;
using NotificationService.Persistence.Data;

namespace NotificationService.Persistence.Repositories;

/// <summary>
/// Repository for NotificationTemplate entity persistence operations.
/// Each tenant has their own database, so no TenantId filtering is needed.
/// </summary>
internal sealed class NotificationTemplateRepository : Repository<NotificationTemplate, NotificationTemplateId>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(DataContext dataContext)
        : base(dataContext)
    {
    }

    public async Task<NotificationTemplate?> GetByNameAsync(string templateName, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .FirstOrDefaultAsync(t => t.TemplateName.ToLower() == templateName.ToLower(), cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .Where(t => t.IsActive)
            .OrderBy(t => t.TemplateName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string templateName, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<NotificationTemplate>()
            .AnyAsync(t => t.TemplateName.ToLower() == templateName.ToLower(), cancellationToken);
    }

    public async Task<(IReadOnlyList<NotificationTemplate> Items, int TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<NotificationTemplate> query = _dataContext.Set<NotificationTemplate>();

        // Apply active filter if specified
        if (activeOnly)
        {
            query = query.Where(t => t.IsActive);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering and pagination at SQL level
        var items = await query
            .OrderBy(t => t.TemplateName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
