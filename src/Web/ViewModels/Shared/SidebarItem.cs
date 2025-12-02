namespace LicenseManagement.Web.ViewModels.Shared;

/// <summary>
/// Sidebar navigation item
/// </summary>
public class SidebarItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public string Icon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Badge { get; set; }
    public string? BadgeColor { get; set; }
    public string? BadgeText { get; set; }
    public List<SidebarItem> Children { get; set; } = [];
    public bool HasChildren => Children.Count > 0;
}
