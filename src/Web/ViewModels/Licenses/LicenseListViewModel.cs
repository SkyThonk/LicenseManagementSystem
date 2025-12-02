using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Licenses;

/// <summary>
/// License list page view model
/// </summary>
public class LicenseListViewModel : BaseViewModel
{
    public PaginatedList<LicenseItemViewModel> Licenses { get; set; } = new();
    public string? SearchQuery { get; set; }
    public string? StatusFilter { get; set; }
    public string? TypeFilter { get; set; }
    public Guid? TenantId { get; set; }
}

/// <summary>
/// Individual license item for list display
/// </summary>
public class LicenseItemViewModel
{
    public Guid Id { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string TenantName { get; set; } = string.Empty;
}
