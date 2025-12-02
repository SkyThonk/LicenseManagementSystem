namespace TenantService.Contracts.Tenant.BlockTenant;

/// <summary>
/// Request to activate or deactivate a government agency
/// </summary>
public record BlockTenantRequest(
    Guid TenantId,
    bool Activate = false
);

