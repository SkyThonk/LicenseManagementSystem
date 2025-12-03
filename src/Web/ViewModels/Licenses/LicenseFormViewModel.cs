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
    public Guid LicenseTypeId { get; set; }

    // Select list options - populated from API
    public List<SelectListItem> LicenseTypes { get; set; } = [];
}

/// <summary>
/// License type view model
/// </summary>
public class LicenseTypeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal FeeAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create license type view model
/// </summary>
public class CreateLicenseTypeViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Fee amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Fee must be a positive number")]
    public decimal FeeAmount { get; set; }
}
