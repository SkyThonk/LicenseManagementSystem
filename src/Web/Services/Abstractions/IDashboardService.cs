using LicenseManagement.Web.ViewModels.Dashboard;

namespace LicenseManagement.Web.Services.Abstractions;

public interface IDashboardService
{
    Task<DashboardIndexViewModel> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}
