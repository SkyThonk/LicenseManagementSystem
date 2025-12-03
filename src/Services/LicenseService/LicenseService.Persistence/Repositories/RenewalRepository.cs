using Microsoft.EntityFrameworkCore;
using LicenseService.Persistence.Common.Abstractions;
using LicenseService.Persistence.Data;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Domain.Renewals;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;

namespace LicenseService.Persistence.Repositories;

/// <summary>
/// Repository for renewal persistence operations
/// </summary>
internal sealed class RenewalRepository : Repository<Renewal, RenewalId>, IRenewalRepository
{
    public RenewalRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<IEnumerable<Renewal>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Renewal>()
            .Where(r => r.LicenseId == licenseId)
            .OrderByDescending(r => r.RenewalDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Renewal>> GetPendingRenewalsAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Renewal>()
            .Where(r => r.Status == RenewalStatus.Pending)
            .Include(r => r.License)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Renewal> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        LicenseId? licenseId = null,
        string? status = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Renewal> query = _dataContext.Set<Renewal>()
            .Include(r => r.License);

        // Apply filters
        if (licenseId != null)
        {
            query = query.Where(r => r.LicenseId == licenseId);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RenewalStatus>(status, true, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "renewaldate" => sortDescending ? query.OrderByDescending(r => r.RenewalDate) : query.OrderBy(r => r.RenewalDate),
            "processedat" => sortDescending ? query.OrderByDescending(r => r.ProcessedAt) : query.OrderBy(r => r.ProcessedAt),
            "status" => sortDescending ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status),
            _ => sortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
