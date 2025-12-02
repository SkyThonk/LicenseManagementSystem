namespace LicenseService.Domain.Common.Enums;

/// <summary>
/// Represents the status of a license application
/// </summary>
public enum LicenseStatus
{
    Draft,
    Submitted,
    UnderReview,
    PendingDocuments,
    Approved,
    Rejected,
    Expired,
    Suspended,
    Revoked,
    Cancelled
}
