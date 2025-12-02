using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Payments;

/// <summary>
/// Payment list page view model
/// </summary>
public class PaymentListViewModel : BaseViewModel
{
    public PaginatedList<PaymentItemViewModel> Payments { get; set; } = new();
    public string? SearchQuery { get; set; }
    public string? StatusFilter { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Individual payment item for list display
/// </summary>
public class PaymentItemViewModel
{
    public Guid Id { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
}
