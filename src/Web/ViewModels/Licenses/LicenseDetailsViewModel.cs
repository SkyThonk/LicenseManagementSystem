using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Licenses;

/// <summary>
/// License details page view model
/// </summary>
public class LicenseDetailsViewModel : BaseViewModel
{
    public Guid Id { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? RejectionReason { get; set; }

    // Applicant Details
    public ApplicantViewModel Applicant { get; set; } = new();

    // Tenant Details
    public string TenantName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }

    // Related Documents
    public List<LicenseDocumentViewModel> Documents { get; set; } = [];

    // Payment History
    public List<LicensePaymentViewModel> Payments { get; set; } = [];

    // Workflow History
    public List<WorkflowHistoryItem> WorkflowHistory { get; set; } = [];
}

public class ApplicantViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public record LicenseDocumentViewModel(
    Guid Id,
    string FileName,
    string DocumentType,
    long SizeKb,
    DateTime UploadedAt
);

public record LicensePaymentViewModel(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    DateTime Date,
    string? ReferenceNumber
);

public record WorkflowHistoryItem(
    string Action,
    string PerformedBy,
    DateTime Timestamp,
    string? Notes
);
