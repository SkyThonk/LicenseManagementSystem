using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.ViewComponents;

public class ModalViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ModalViewModel model)
    {
        return View(model);
    }
}

public class ModalViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Size { get; set; } // sm, md, lg, xl
    public bool ShowFooter { get; set; } = true;
    public bool StaticBackdrop { get; set; } = false;
    public string? SubmitButtonText { get; set; } = "Save";
    public string? CancelButtonText { get; set; } = "Cancel";
    public string SubmitButtonClass { get; set; } = "btn-primary";
}
