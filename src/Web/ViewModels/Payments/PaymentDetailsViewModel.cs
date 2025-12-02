using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Payments;

/// <summary>
/// Payment details page view model
/// </summary>
public class PaymentDetailsViewModel : BaseViewModel
{
    public Guid Id { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ErrorMessage { get; set; }

    // Related Info
    public string TenantName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string? LicenseNumber { get; set; }
    public Guid? LicenseId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;

    // Can perform actions
    public bool CanProcess => Status == "Pending";
    public bool CanCancel => Status == "Pending";
    public bool CanRefund => Status == "Paid";
}
