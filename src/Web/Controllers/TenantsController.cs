using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Tenants;
using LicenseManagement.Web.Filters;

namespace LicenseManagement.Web.Controllers;

[RequireRole("TenantAdmin")]
public class TenantsController : Controller
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ITenantService tenantService, ILogger<TenantsController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Tenants";
        
        try
        {
            var viewModel = await _tenantService.GetTenantsAsync(page, pageSize, search, cancellationToken);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tenants");
            TempData["ErrorMessage"] = "Failed to load tenants.";
            return View(new TenantListViewModel());
        }
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Tenant Details";
        
        try
        {
            var viewModel = await _tenantService.GetTenantByIdAsync(id, cancellationToken);
            
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tenant details for {TenantId}", id);
            TempData["ErrorMessage"] = "Failed to load tenant details.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Create Tenant";
        return View(new TenantFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TenantFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Create Tenant";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var success = await _tenantService.CreateTenantAsync(model, cancellationToken);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Tenant created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create tenant.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            TempData["ErrorMessage"] = "An error occurred while creating the tenant.";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Tenant";

        try
        {
            var tenant = await _tenantService.GetTenantByIdAsync(id, cancellationToken);
            
            if (tenant == null)
            {
                TempData["ErrorMessage"] = "Tenant not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new TenantFormViewModel
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Email = tenant.Email,
                Phone = tenant.Phone,
                Address = tenant.Address,
                City = tenant.City,
                Country = tenant.Country
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tenant for edit {TenantId}", id);
            TempData["ErrorMessage"] = "Failed to load tenant.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TenantFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit Tenant";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var success = await _tenantService.UpdateTenantAsync(id, model, cancellationToken);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Tenant updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["ErrorMessage"] = "Failed to update tenant.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant {TenantId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the tenant.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _tenantService.DeleteTenantAsync(id, cancellationToken);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Tenant deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete tenant.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant {TenantId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tenant.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _tenantService.ActivateTenantAsync(id, cancellationToken);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Tenant activated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to activate tenant.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating tenant {TenantId}", id);
            TempData["ErrorMessage"] = "An error occurred while activating the tenant.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _tenantService.DeactivateTenantAsync(id, cancellationToken);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Tenant deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to deactivate tenant.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating tenant {TenantId}", id);
            TempData["ErrorMessage"] = "An error occurred while deactivating the tenant.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
