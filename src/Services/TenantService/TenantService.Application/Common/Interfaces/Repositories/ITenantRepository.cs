using TenantEntity = TenantService.Domain.Tenant.Tenant;
using TenantIdEntity = TenantService.Domain.Tenant.TenantId;

namespace TenantService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for government agency (tenant) entities
/// </summary>
public interface ITenantRepository
{
    void Add(TenantEntity tenant);
    void Update(TenantEntity tenant);
    Task<TenantEntity?> GetByIdAsync(TenantIdEntity id, CancellationToken cancellationToken = default);
    Task<TenantEntity?> GetByAgencyCodeAsync(string agencyCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAgencyCodeAsync(string agencyCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantEntity>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<TenantEntity> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}

