using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for LicenseDocument entities.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public interface ILicenseDocumentRepository
{
    void Add(LicenseDocument document);
    void Update(LicenseDocument document);
    void Delete(LicenseDocument document);
    Task<LicenseDocument?> GetByIdAsync(LicenseDocumentId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseDocument>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseDocument>> GetAllAsync(CancellationToken cancellationToken = default);
}
