using System.ComponentModel.DataAnnotations;
using LicenseManagement.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LicenseManagement.Web.ViewModels.Documents;

/// <summary>
/// Document upload form view model
/// </summary>
public class DocumentUploadViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Please select a license")]
    [Display(Name = "License")]
    public Guid LicenseId { get; set; }

    [Required(ErrorMessage = "Document type is required")]
    [Display(Name = "Document Type")]
    public string DocumentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a file")]
    [Display(Name = "File")]
    public IFormFile? File { get; set; }

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    // Select list options
    public List<SelectListItem> Licenses { get; set; } = [];
    public List<SelectListItem> DocumentTypes { get; set; } = [];
}
