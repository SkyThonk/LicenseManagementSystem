using LicenseService.Domain.LicenseStatusHistory;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for LicenseStatusHistoryEntry entities
/// </summary>
public interface ILicenseStatusHistoryRepository
{
    void Add(LicenseStatusHistoryEntry entry);
    Task<IEnumerable<LicenseStatusHistoryEntry>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseStatusHistoryEntry>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<LicenseStatusHistoryEntry> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        LicenseId? licenseId = null,
        Guid? tenantId = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
}
