using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewComponents;

public class PaginationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PaginationViewModel model)
    {
        return View(model);
    }
}

public class PaginationViewModel
{
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string>? QueryParameters { get; set; }
    
    public int StartItem => TotalItems == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
    public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    
    public string GetPageUrl(int page)
    {
        var queryParams = new Dictionary<string, string>(QueryParameters ?? new Dictionary<string, string>())
        {
            ["page"] = page.ToString()
        };
        
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{BaseUrl}?{queryString}";
    }
}
