using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewComponents;

public class TableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TableViewModel model)
    {
        return View(model);
    }
}

public class TableViewModel
{
    public List<TableColumn> Columns { get; set; } = new();
    public bool Selectable { get; set; } = false;
    public bool Striped { get; set; } = true;
    public bool ShowSearch { get; set; } = true;
    public string? SearchPlaceholder { get; set; } = "Search...";
    public string? EmptyTitle { get; set; } = "No data found";
    public string? EmptyMessage { get; set; } = "There are no records to display.";
    public string? EmptyIcon { get; set; } = "fas fa-inbox";
}
