namespace LicenseService.Contracts.Renewals.CreateRenewal;

/// <summary>
/// Response after creating a new renewal
/// </summary>
public record CreateRenewalResponse(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime CreatedAt
);
