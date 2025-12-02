namespace LicenseService.Domain.Common.Enums;

/// <summary>
/// Represents the status of a license renewal
/// </summary>
public enum RenewalStatus
{
    Pending,
    Processing,
    Approved,
    Rejected,
    Completed,
    Failed
}
