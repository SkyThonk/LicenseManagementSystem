using Microsoft.EntityFrameworkCore;
using LicenseService.Persistence.Common.Abstractions;
using LicenseService.Persistence.Data;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Domain.LicenseStatusHistory;
using LicenseService.Domain.Licenses;

namespace LicenseService.Persistence.Repositories;

/// <summary>
/// Repository for license status history persistence operations
/// </summary>
internal sealed class LicenseStatusHistoryRepository : Repository<LicenseStatusHistoryEntry, LicenseStatusHistoryId>, ILicenseStatusHistoryRepository
{
    public LicenseStatusHistoryRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<IEnumerable<LicenseStatusHistoryEntry>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseStatusHistoryEntry>()
            .Where(h => h.LicenseId == licenseId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LicenseStatusHistoryEntry>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseStatusHistoryEntry>()
            .Where(h => h.TenantId == tenantId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<LicenseStatusHistoryEntry> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        LicenseId? licenseId = null,
        Guid? tenantId = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<LicenseStatusHistoryEntry> query = _dataContext.Set<LicenseStatusHistoryEntry>();

        // Apply filters
        if (licenseId != null)
        {
            query = query.Where(h => h.LicenseId == licenseId);
        }

        if (tenantId.HasValue)
        {
            query = query.Where(h => h.TenantId == tenantId.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "changedat" => sortDescending ? query.OrderByDescending(h => h.ChangedAt) : query.OrderBy(h => h.ChangedAt),
            "oldstatus" => sortDescending ? query.OrderByDescending(h => h.OldStatus) : query.OrderBy(h => h.OldStatus),
            "newstatus" => sortDescending ? query.OrderByDescending(h => h.NewStatus) : query.OrderBy(h => h.NewStatus),
            _ => sortDescending ? query.OrderBy(h => h.ChangedAt) : query.OrderByDescending(h => h.ChangedAt) // Default to most recent first
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
