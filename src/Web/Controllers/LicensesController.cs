using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Licenses;
using LicenseManagement.Web.Filters;

namespace LicenseManagement.Web.Controllers;

[RequireAuthentication]
[Route("licenses")]
public class LicensesController : Controller
{
    private readonly ILicenseService _licenseService;
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(
        ILicenseService licenseService,
        ILogger<LicensesController> logger)
    {
        _licenseService = licenseService;
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("index")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? status = null)
    {
        var viewModel = await _licenseService.GetLicensesAsync(page, pageSize, search, status);
        
        // Get license types for the create dialog
        var licenseTypes = await _licenseService.GetLicenseTypesAsync();
        ViewBag.LicenseTypes = licenseTypes;
        
        return View(viewModel);
    }

    [HttpGet("details/{id:guid}")]
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

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var licenseTypes = await _licenseService.GetLicenseTypesAsync();
        var viewModel = new LicenseFormViewModel
        {
            LicenseTypes = licenseTypes.Select(lt => new SelectListItem
            {
                Value = lt.Id.ToString(),
                Text = $"{lt.Name} (${lt.FeeAmount:F2})"
            }).ToList()
        };
        
        // Add empty option at start
        viewModel.LicenseTypes.Insert(0, new SelectListItem { Value = "", Text = "Select license type" });
        
        return View(viewModel);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LicenseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLicenseTypesAsync(model);
            return View(model);
        }

        var result = await _licenseService.CreateLicenseAsync(model);
        if (result)
        {
            TempData["Success"] = "License created successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Failed to create license. Please try again.";
        await PopulateLicenseTypesAsync(model);
        return View(model);
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var license = await _licenseService.GetLicenseByIdAsync(id);
        if (license == null)
        {
            TempData["Error"] = "License not found.";
            return RedirectToAction(nameof(Index));
        }

        var licenseTypes = await _licenseService.GetLicenseTypesAsync();
        var viewModel = new LicenseFormViewModel
        {
            Id = license.Id,
            LicenseTypeId = Guid.TryParse(license.LicenseType, out var typeId) ? typeId : Guid.Empty,
            LicenseTypes = licenseTypes.Select(lt => new SelectListItem
            {
                Value = lt.Id.ToString(),
                Text = $"{lt.Name} (${lt.FeeAmount:F2})"
            }).ToList()
        };
        
        viewModel.LicenseTypes.Insert(0, new SelectListItem { Value = "", Text = "Select license type" });

        return View(viewModel);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LicenseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLicenseTypesAsync(model);
            return View(model);
        }

        var result = await _licenseService.UpdateLicenseAsync(id, model);
        if (result)
        {
            TempData["Success"] = "License updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Error"] = "Failed to update license. Please try again.";
        await PopulateLicenseTypesAsync(model);
        return View(model);
    }

    [HttpPost("delete/{id:guid}")]
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

    [HttpPost("activate/{id:guid}")]
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

    [HttpPost("deactivate/{id:guid}")]
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

    [HttpPost("renew/{id:guid}")]
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

    // License Type endpoints
    [HttpPost("types/create")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreateLicenseType([FromBody] CreateLicenseTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Invalid data" });
        }

        var result = await _licenseService.CreateLicenseTypeAsync(model);
        if (result)
        {
            return Ok(new { success = true, message = "License type created successfully" });
        }

        return BadRequest(new { success = false, message = "Failed to create license type" });
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetLicenseTypes()
    {
        var licenseTypes = await _licenseService.GetLicenseTypesAsync();
        return Ok(licenseTypes);
    }

    private async Task PopulateLicenseTypesAsync(LicenseFormViewModel model)
    {
        var licenseTypes = await _licenseService.GetLicenseTypesAsync();
        model.LicenseTypes = licenseTypes.Select(lt => new SelectListItem
        {
            Value = lt.Id.ToString(),
            Text = $"{lt.Name} (${lt.FeeAmount:F2})"
        }).ToList();
        model.LicenseTypes.Insert(0, new SelectListItem { Value = "", Text = "Select license type" });
    }
}
