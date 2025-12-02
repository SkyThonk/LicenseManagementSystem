using Microsoft.EntityFrameworkCore;
using LicenseService.Persistence.Common.Abstractions;
using LicenseService.Persistence.Data;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Persistence.Repositories;

/// <summary>
/// Repository for license type persistence operations
/// </summary>
internal sealed class LicenseTypeRepository : Repository<LicenseType, LicenseTypeId>, ILicenseTypeRepository
{
    public LicenseTypeRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<IEnumerable<LicenseType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseType>()
            .Where(lt => lt.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseType>()
            .AnyAsync(lt => lt.TenantId == tenantId && lt.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<(IEnumerable<LicenseType> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? tenantId = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<LicenseType> query = _dataContext.Set<LicenseType>();

        // Apply filters
        if (tenantId.HasValue)
        {
            query = query.Where(lt => lt.TenantId == tenantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(lt => 
                lt.Name.ToLower().Contains(term) || 
                (lt.Description != null && lt.Description.ToLower().Contains(term)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ? query.OrderByDescending(lt => lt.Name) : query.OrderBy(lt => lt.Name),
            "feeamount" => sortDescending ? query.OrderByDescending(lt => lt.FeeAmount) : query.OrderBy(lt => lt.FeeAmount),
            _ => sortDescending ? query.OrderByDescending(lt => lt.CreatedAt) : query.OrderBy(lt => lt.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
