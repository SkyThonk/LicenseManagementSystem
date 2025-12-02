namespace LicenseService.Contracts.Renewals;

public record CreateRenewalRequest(
    Guid TenantId,
    Guid LicenseId,
    DateTime RenewalDate
);
