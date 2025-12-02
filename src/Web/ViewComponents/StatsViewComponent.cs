using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.ViewComponents;

public class StatsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(StatsCardViewModel model)
    {
        return View(model);
    }
}

public class StatsCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-chart-bar";
    public string IconColor { get; set; } = "primary"; // primary, success, warning, danger, info
    public string? ChangeValue { get; set; }
    public bool IsPositiveChange { get; set; } = true;
    public string? ChangeText { get; set; }
    public int? ProgressValue { get; set; }
    public string? ProgressColor { get; set; }
    public string? ProgressText { get; set; }
    public string? Url { get; set; }
}
