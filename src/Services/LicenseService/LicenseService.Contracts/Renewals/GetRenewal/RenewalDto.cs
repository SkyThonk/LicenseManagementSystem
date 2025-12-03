namespace LicenseService.Contracts.Renewals.GetRenewal;

/// <summary>
/// Renewal details DTO.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record RenewalDto(
    Guid Id,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
