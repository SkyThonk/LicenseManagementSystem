namespace LicenseService.Contracts.Licenses.UpdateLicenseStatus;

/// <summary>
/// Response after updating license status
/// </summary>
public record UpdateLicenseStatusResponse(
    Guid Id,
    string Status,
    DateTime? ApprovedAt,
    DateTime? ExpiryDate,
    DateTime UpdatedAt
);
