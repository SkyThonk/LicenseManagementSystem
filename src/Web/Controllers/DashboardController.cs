using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.Filters;

namespace LicenseManagement.Web.Controllers;

[RequireAuthentication]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IAuthService _authService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService, 
        IAuthService authService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";
        
        // Pass user info to view
        var user = _authService.GetCurrentUser();
        ViewBag.CurrentUser = user;
        
        try
        {
            var viewModel = await _dashboardService.GetDashboardDataAsync(cancellationToken);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["ErrorMessage"] = "Failed to load dashboard data.";
            return View();
        }
    }
}
