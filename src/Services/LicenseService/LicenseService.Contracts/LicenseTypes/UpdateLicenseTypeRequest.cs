namespace LicenseService.Contracts.LicenseTypes;

public record UpdateLicenseTypeRequest(
    string Name,
    string? Description,
    decimal FeeAmount
);
