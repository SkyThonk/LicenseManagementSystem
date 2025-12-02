using LicenseManagement.Web.ViewModels.Tenants;

namespace LicenseManagement.Web.Services.Abstractions;

public interface ITenantService
{
    Task<TenantListViewModel> GetTenantsAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default);
    Task<TenantDetailsViewModel?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CreateTenantAsync(TenantFormViewModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateTenantAsync(Guid id, TenantFormViewModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ActivateTenantAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeactivateTenantAsync(Guid id, CancellationToken cancellationToken = default);
}
