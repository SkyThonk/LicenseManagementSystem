using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Tenants;

/// <summary>
/// Tenant details page view model
/// </summary>
public class TenantDetailsViewModel : BaseViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int LicenseCount { get; set; }
    public decimal TotalPayments { get; set; }

    public List<StatCardViewModel> Stats { get; set; } = [];
    public List<TenantLicenseViewModel> RecentLicenses { get; set; } = [];
    public List<TenantPaymentViewModel> RecentPayments { get; set; } = [];
}

public record TenantLicenseViewModel(
    Guid Id,
    string LicenseNumber,
    string LicenseType,
    string Status,
    DateTime ExpiryDate
);

public record TenantPaymentViewModel(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    DateTime Date
);
