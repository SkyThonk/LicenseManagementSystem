using System.ComponentModel.DataAnnotations;
using LicenseManagement.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LicenseManagement.Web.ViewModels.Licenses;

/// <summary>
/// Create/Edit license form view model
/// </summary>
public class LicenseFormViewModel : BaseViewModel
{
    public Guid? Id { get; set; }
    public bool IsEdit => Id.HasValue;

    [Required(ErrorMessage = "License type is required")]
    [Display(Name = "License Type")]
    public string LicenseType { get; set; } = string.Empty;

    [Display(Name = "Tenant")]
    public Guid? TenantId { get; set; }

    // Applicant Information
    [Required(ErrorMessage = "Applicant name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
    [Display(Name = "Applicant Name")]
    public string ApplicantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string ApplicantEmail { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone")]
    public string? ApplicantPhone { get; set; }

    [Display(Name = "Address")]
    public string? ApplicantAddress { get; set; }

    [Display(Name = "Notes")]
    [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
    public string? Notes { get; set; }

    // Select list options
    public List<SelectListItem> LicenseTypes { get; set; } = [];
    public List<SelectListItem> Tenants { get; set; } = [];
}
