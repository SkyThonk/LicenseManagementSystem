namespace LicenseManagement.Web.ViewModels.Shared;

/// <summary>
/// Base view model with common properties
/// </summary>
public abstract class BaseViewModel
{
    public string? PageTitle { get; set; }
    public string? PageDescription { get; set; }
    public List<BreadcrumbItem> Breadcrumbs { get; set; } = [];
}

/// <summary>
/// Breadcrumb navigation item
/// </summary>
public record BreadcrumbItem(string Label, string? Url = null, bool IsActive = false);
