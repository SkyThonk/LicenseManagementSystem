using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for LicenseType entities
/// </summary>
public interface ILicenseTypeRepository
{
    void Add(LicenseType licenseType);
    void Update(LicenseType licenseType);
    Task<LicenseType?> GetByIdAsync(LicenseTypeId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task<(IEnumerable<LicenseType> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? tenantId = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
}
