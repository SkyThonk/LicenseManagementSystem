using LicenseService.Domain.Licenses;
using LicenseService.Domain.Renewals;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Renewal entities.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public interface IRenewalRepository
{
    void Add(Renewal renewal);
    void Update(Renewal renewal);
    Task<Renewal?> GetByIdAsync(RenewalId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Renewal>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Renewal>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Renewal>> GetPendingRenewalsAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Renewal> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        LicenseId? licenseId = null,
        string? status = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
}
