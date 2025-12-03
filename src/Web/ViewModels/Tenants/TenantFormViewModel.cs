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

    [Required(ErrorMessage = "Agency code is required")]
    [StringLength(50, ErrorMessage = "Agency code cannot exceed 50 characters")]
    [Display(Name = "Agency Code")]
    public string AgencyCode { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    [Display(Name = "Address Line")]
    public string AddressLine { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
    [Display(Name = "State")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country code is required")]
    [StringLength(5, ErrorMessage = "Country code cannot exceed 5 characters")]
    [Display(Name = "Country Code")]
    public string CountryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    // Admin user fields for registration
    [Display(Name = "Admin First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Admin Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Admin Password")]
    public string? Password { get; set; }

    [Display(Name = "Logo URL")]
    public string? Logo { get; set; }
}
