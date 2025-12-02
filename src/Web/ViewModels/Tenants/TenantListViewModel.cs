using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Tenants;

/// <summary>
/// Tenant list page view model
/// </summary>
public class TenantListViewModel : BaseViewModel
{
    public PaginatedList<TenantListItemViewModel> Tenants { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Individual tenant item for list display
/// </summary>
public class TenantListItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public int LicenseCount { get; set; }
}
