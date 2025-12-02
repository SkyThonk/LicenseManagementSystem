using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewComponents;

public class AlertViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AlertViewModel model)
    {
        return View(model);
    }
}
