using Microsoft.EntityFrameworkCore;
using TenantService.Persistence.Common.Abstractions;
using TenantService.Persistence.Data;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Domain.Tenant;

namespace TenantService.Persistence.Repositories;

/// <summary>
/// Repository for government agency (tenant) persistence operations
/// </summary>
internal sealed class TenantRepository : Repository<Tenant, TenantId>, ITenantRepository
{
    public TenantRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<Tenant?> GetByAgencyCodeAsync(string agencyCode, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.AgencyCode.ToLower() == agencyCode.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByAgencyCodeAsync(string agencyCode, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Tenant>()
            .AnyAsync(t => t.AgencyCode.ToLower() == agencyCode.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Retrieve paginated list of tenants with optional search, filtering and sorting
    /// </summary>
    public async Task<(IEnumerable<Tenant> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Tenant> query = _dataContext.Set<Tenant>();

        // Apply active status filter if provided
        if (isActive.HasValue)
        {
            query = query.Where(t => t.IsActive == isActive.Value);
        }

        // Apply search filter if provided (case-insensitive contains)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lower = searchTerm.ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(lower) ||
                                     t.AgencyCode.ToLower().Contains(lower) ||
                                     t.Email.ToLower().Contains(lower));
        }

        // Apply sorting based on requested column and direction
        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
            "agencycode" => sortDescending ? query.OrderByDescending(t => t.AgencyCode) : query.OrderBy(t => t.AgencyCode),
            "email" => sortDescending ? query.OrderByDescending(t => t.Email) : query.OrderBy(t => t.Email),
            "createdat" => sortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            "updatedat" => sortDescending ? query.OrderByDescending(t => t.UpdatedAt) : query.OrderBy(t => t.UpdatedAt),
            _ => query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

