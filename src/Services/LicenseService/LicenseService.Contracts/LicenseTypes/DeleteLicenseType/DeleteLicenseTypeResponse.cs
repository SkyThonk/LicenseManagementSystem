namespace LicenseService.Contracts.LicenseTypes.DeleteLicenseType;

/// <summary>
/// Response after deleting a license type
/// </summary>
public record DeleteLicenseTypeResponse(
    Guid Id,
    bool Success,
    string Message
);
