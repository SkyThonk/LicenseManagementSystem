using System.ComponentModel.DataAnnotations;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Tenants;

/// <summary>
/// Create/Edit tenant form view model
/// </summary>
public class TenantFormViewModel : BaseViewModel
{
    public Guid? Id { get; set; }
    public bool IsEdit => Id.HasValue;

    [Required(ErrorMessage = "Organization name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Organization name must be between 2 and 200 characters")]
    [Display(Name = "Organization Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    [Display(Name = "Street Address")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    [Display(Name = "City")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    [Display(Name = "Country")]
    public string? Country { get; set; }
}
