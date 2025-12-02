using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Documents;

namespace LicenseManagement.Web.Controllers;

public class DocumentsController : Controller
{
    private readonly IDocumentService _documentService;
    private readonly ILicenseService _licenseService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        ILicenseService licenseService,
        ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _licenseService = licenseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null)
    {
        var viewModel = await _documentService.GetDocumentsAsync(page, pageSize, search);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Upload()
    {
        var viewModel = new DocumentUploadViewModel
        {
            Licenses = await GetLicensesSelectListAsync(),
            DocumentTypes = GetDocumentTypes()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(DocumentUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Licenses = await GetLicensesSelectListAsync();
            model.DocumentTypes = GetDocumentTypes();
            return View(model);
        }

        var result = await _documentService.UploadDocumentAsync(model);
        if (result)
        {
            TempData["Success"] = "Document uploaded successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Failed to upload document. Please try again.";
        model.Licenses = await GetLicensesSelectListAsync();
        model.DocumentTypes = GetDocumentTypes();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _documentService.DeleteDocumentAsync(id);
        if (result)
        {
            TempData["Success"] = "Document deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete document.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var url = await _documentService.GetDownloadUrlAsync(id);
        if (string.IsNullOrEmpty(url))
        {
            TempData["Error"] = "Document not found.";
            return RedirectToAction(nameof(Index));
        }
        return Redirect(url);
    }

    private async Task<List<SelectListItem>> GetLicensesSelectListAsync()
    {
        var licenses = await _licenseService.GetLicensesAsync(1, 100);
        var items = licenses.Licenses.Items.Select(l => new SelectListItem
        {
            Value = l.Id.ToString(),
            Text = $"{l.LicenseNumber} - {l.ApplicantName}"
        }).ToList();
        
        items.Insert(0, new SelectListItem { Value = "", Text = "Select license" });
        return items;
    }

    private List<SelectListItem> GetDocumentTypes()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Select document type" },
            new SelectListItem { Value = "License Agreement", Text = "License Agreement" },
            new SelectListItem { Value = "Identity Document", Text = "Identity Document" },
            new SelectListItem { Value = "Insurance", Text = "Insurance" },
            new SelectListItem { Value = "Contract", Text = "Contract" },
            new SelectListItem { Value = "Other", Text = "Other" }
        };
    }
}
