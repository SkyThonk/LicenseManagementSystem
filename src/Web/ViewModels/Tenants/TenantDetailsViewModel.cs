using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Tenants;

/// <summary>
/// Tenant details page view model
/// </summary>
public class TenantDetailsViewModel : BaseViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgencyCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public bool IsActive { get; set; }
    public string Status => IsActive ? "Active" : "Inactive";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Address
    public string? AddressLineOne { get; set; }
    public string? AddressLineTwo { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    
    // Phone
    public string? PhoneCountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhoneFullNumber { get; set; }

    public List<StatCardViewModel> Stats { get; set; } = [];
}
