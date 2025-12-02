using LicenseManagement.Web.ViewModels.Licenses;

namespace LicenseManagement.Web.Services.Abstractions;

public interface ILicenseService
{
    Task<LicenseListViewModel> GetLicensesAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, CancellationToken cancellationToken = default);
    Task<LicenseDetailsViewModel?> GetLicenseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CreateLicenseAsync(LicenseFormViewModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateLicenseAsync(Guid id, LicenseFormViewModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteLicenseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ActivateLicenseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeactivateLicenseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RenewLicenseAsync(Guid id, DateTime newExpirationDate, CancellationToken cancellationToken = default);
}
