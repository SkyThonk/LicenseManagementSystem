using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string title)
    {
        var model = new HeaderViewModel
        {
            Title = title,
            UnreadNotificationsCount = 3 // TODO: Get from service
        };
        
        return View(model);
    }
}

public class HeaderViewModel
{
    public string Title { get; set; } = string.Empty;
    public int UnreadNotificationsCount { get; set; }
}
