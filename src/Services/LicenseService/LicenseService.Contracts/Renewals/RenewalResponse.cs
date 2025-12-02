namespace LicenseService.Contracts.Renewals;

public record RenewalResponse(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
