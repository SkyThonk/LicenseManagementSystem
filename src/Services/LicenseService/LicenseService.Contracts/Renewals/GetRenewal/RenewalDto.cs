namespace LicenseService.Contracts.Renewals.GetRenewal;

/// <summary>
/// Renewal details DTO
/// </summary>
public record RenewalDto(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
