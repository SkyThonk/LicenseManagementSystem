namespace TenantService.Contracts.Tenant.BlockTenant;

/// <summary>
/// Response after activating/deactivating a government agency
/// </summary>
public record BlockTenantResponse(
    bool Success,
    string Message,
    bool IsActive
);

