namespace TenantService.Contracts.Tenant.RegisterTenant;

/// <summary>
/// Response after successfully registering a government agency
/// </summary>
public record RegisterTenantResponse(
    Guid TenantId,
    string Name,
    string AgencyCode,
    DateTime CreatedAt
);

