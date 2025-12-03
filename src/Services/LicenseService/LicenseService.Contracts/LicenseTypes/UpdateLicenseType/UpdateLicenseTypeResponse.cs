namespace LicenseService.Contracts.LicenseTypes.UpdateLicenseType;

/// <summary>
/// Response after updating a license type
/// </summary>
public record UpdateLicenseTypeResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime UpdatedAt
);
