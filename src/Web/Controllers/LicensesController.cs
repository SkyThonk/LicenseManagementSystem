using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Licenses;
using LicenseManagement.Web.Filters;

namespace LicenseManagement.Web.Controllers;

[RequireAuthentication]
public class LicensesController : Controller
{
    private readonly ILicenseService _licenseService;
    private readonly ITenantService _tenantService;
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(
        ILicenseService licenseService,
        ITenantService tenantService,
        ILogger<LicensesController> logger)
    {
        _licenseService = licenseService;
        _tenantService = tenantService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? status = null)
    {
        var viewModel = await _licenseService.GetLicensesAsync(page, pageSize, search, status);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var viewModel = await _licenseService.GetLicenseByIdAsync(id);
        if (viewModel == null)
        {
            TempData["Error"] = "License not found.";
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new LicenseFormViewModel
        {
            LicenseTypes = GetLicenseTypes(),
            Tenants = await GetTenantsSelectListAsync()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LicenseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.LicenseTypes = GetLicenseTypes();
            model.Tenants = await GetTenantsSelectListAsync();
            return View(model);
        }

        var result = await _licenseService.CreateLicenseAsync(model);
        if (result)
        {
            TempData["Success"] = "License created successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Failed to create license. Please try again.";
        model.LicenseTypes = GetLicenseTypes();
        model.Tenants = await GetTenantsSelectListAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var license = await _licenseService.GetLicenseByIdAsync(id);
        if (license == null)
        {
            TempData["Error"] = "License not found.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new LicenseFormViewModel
        {
            Id = license.Id,
            LicenseType = license.LicenseType,
            TenantId = license.TenantId,
            ApplicantName = license.Applicant.FullName,
            ApplicantEmail = license.Applicant.Email,
            ApplicantPhone = license.Applicant.Phone,
            ApplicantAddress = license.Applicant.Address,
            LicenseTypes = GetLicenseTypes(),
            Tenants = await GetTenantsSelectListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LicenseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.LicenseTypes = GetLicenseTypes();
            model.Tenants = await GetTenantsSelectListAsync();
            return View(model);
        }

        var result = await _licenseService.UpdateLicenseAsync(id, model);
        if (result)
        {
            TempData["Success"] = "License updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Error"] = "Failed to update license. Please try again.";
        model.LicenseTypes = GetLicenseTypes();
        model.Tenants = await GetTenantsSelectListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _licenseService.DeleteLicenseAsync(id);
        if (result)
        {
            TempData["Success"] = "License deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete license.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _licenseService.ActivateLicenseAsync(id);
        if (result)
        {
            TempData["Success"] = "License activated successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to activate license.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _licenseService.DeactivateLicenseAsync(id);
        if (result)
        {
            TempData["Success"] = "License deactivated successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to deactivate license.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(Guid id, DateTime expirationDate)
    {
        var result = await _licenseService.RenewLicenseAsync(id, expirationDate);
        if (result)
        {
            TempData["Success"] = "License renewed successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to renew license.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private List<SelectListItem> GetLicenseTypes()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Select license type" },
            new SelectListItem { Value = "Enterprise", Text = "Enterprise" },
            new SelectListItem { Value = "Professional", Text = "Professional" },
            new SelectListItem { Value = "Basic", Text = "Basic" },
            new SelectListItem { Value = "Trial", Text = "Trial" }
        };
    }

    private async Task<List<SelectListItem>> GetTenantsSelectListAsync()
    {
        var tenants = await _tenantService.GetTenantsAsync(1, 100);
        var items = tenants.Tenants.Items.Select(t => new SelectListItem
        {
            Value = t.Id.ToString(),
            Text = t.Name
        }).ToList();
        
        items.Insert(0, new SelectListItem { Value = "", Text = "Select tenant" });
        return items;
    }
}
