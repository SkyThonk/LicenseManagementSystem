using Microsoft.EntityFrameworkCore;
using LicenseService.Persistence.Common.Abstractions;
using LicenseService.Persistence.Data;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.Licenses;

namespace LicenseService.Persistence.Repositories;

/// <summary>
/// Repository for license document persistence operations
/// </summary>
internal sealed class LicenseDocumentRepository : Repository<LicenseDocument, LicenseDocumentId>, ILicenseDocumentRepository
{
    public LicenseDocumentRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<IEnumerable<LicenseDocument>> GetByLicenseIdAsync(LicenseId licenseId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseDocument>()
            .Where(ld => ld.LicenseId == licenseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LicenseDocument>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<LicenseDocument>()
            .Where(ld => ld.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }
}
