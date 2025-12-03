namespace LicenseService.Contracts.Renewals.ProcessRenewal;

/// <summary>
/// Response after processing a renewal
/// </summary>
public record ProcessRenewalResponse(
    Guid Id,
    string Status,
    DateTime ProcessedAt,
    DateTime? NewExpiryDate
);
