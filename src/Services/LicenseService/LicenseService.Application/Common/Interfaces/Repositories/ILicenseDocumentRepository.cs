using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for LicenseDocument entities
/// </summary>
public interface ILicenseDocumentRepository
{
    void Add(LicenseDocument document);
    void Update(LicenseDocument document);
    void Delete(LicenseDocument document);
    Task<LicenseDocument?> GetByIdAsync(LicenseDocumentId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseDocument>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LicenseDocument>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
