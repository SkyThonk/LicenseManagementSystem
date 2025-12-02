using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for License entities
/// </summary>
public interface ILicenseRepository
{
    void Add(License license);
    void Update(License license);
    Task<License?> GetByIdAsync(LicenseId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByApplicantIdAsync(Guid applicantId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<License> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? tenantId = null,
        Guid? applicantId = null,
        string? status = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
}
