using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.ViewComponents;

public class CardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CardViewModel model)
    {
        return View(model);
    }
}

public class CardViewModel
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? CssClass { get; set; }
    public bool ShowHeader { get; set; } = true;
    public string? HeaderActionsHtml { get; set; }
}
