using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.Services.Abstractions;

namespace LicenseManagement.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";
        
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
