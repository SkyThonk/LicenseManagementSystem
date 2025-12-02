namespace LicenseManagement.Web.ViewModels.Shared;

/// <summary>
/// Statistics card view model
/// </summary>
public class StatCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = "0";
    public string? Icon { get; set; }
    public string? IconColor { get; set; } = "primary";
    public string? Description { get; set; }
    public string? ChangePercentage { get; set; }
    public bool IsPositiveChange { get; set; } = true;
    public string? Period { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
}
