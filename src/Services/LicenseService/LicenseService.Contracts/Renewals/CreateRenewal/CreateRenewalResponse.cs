namespace LicenseService.Contracts.Renewals.CreateRenewal;

/// <summary>
/// Response after creating a new renewal.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record CreateRenewalResponse(
    Guid Id,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime CreatedAt
);
