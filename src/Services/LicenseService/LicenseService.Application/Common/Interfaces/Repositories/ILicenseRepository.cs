using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for License entities.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public interface ILicenseRepository
{
    void Add(License license);
    void Update(License license);
    Task<License?> GetByIdAsync(LicenseId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetByApplicantIdAsync(Guid applicantId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<License> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? applicantId = null,
        string? status = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
}
