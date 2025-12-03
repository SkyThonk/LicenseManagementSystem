using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.ViewModels.Shared;
using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;

namespace LicenseManagement.Web.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IAuthService _authService;

    public SidebarViewComponent(IAuthService authService)
    {
        _authService = authService;
    }

    public IViewComponentResult Invoke()
    {
        var currentPath = HttpContext.Request.Path.Value?.ToLower() ?? "";
        var user = _authService.GetCurrentUser();
        var isTenantAdmin = user?.Role == "TenantAdmin";
        
        var items = new List<SidebarItem>
        {
            new SidebarItem
            {
                Title = "Dashboard",
                Icon = "fas fa-th-large",
                Url = AppRoutes.Dashboard.Index,
                IsActive = currentPath == "/" || currentPath.StartsWith("/dashboard")
            }
        };

        // Only show Tenants menu for TenantAdmin role
        if (isTenantAdmin)
        {
            items.Add(new SidebarItem
            {
                Title = "Tenants",
                Icon = "fas fa-building",
                Url = AppRoutes.Tenants.Index,
                IsActive = currentPath.StartsWith("/tenants"),
                BadgeText = null
            });
        }

        items.AddRange(new[]
        {
            new SidebarItem
            {
                Title = "Licenses",
                Icon = "fas fa-key",
                Url = AppRoutes.Licenses.Index,
                IsActive = currentPath.StartsWith("/licenses")
            },
            new SidebarItem
            {
                Title = "Payments",
                Icon = "fas fa-credit-card",
                Url = AppRoutes.Payments.Index,
                IsActive = currentPath.StartsWith("/payments")
            },
            new SidebarItem
            {
                Title = "Documents",
                Icon = "fas fa-folder-open",
                Url = AppRoutes.Documents.Index,
                IsActive = currentPath.StartsWith("/documents")
            },
            new SidebarItem
            {
                Title = "Notifications",
                Icon = "fas fa-bell",
                Url = AppRoutes.Notifications.Index,
                IsActive = currentPath.StartsWith("/notifications")
            }
        });

        return View(items);
    }
}
