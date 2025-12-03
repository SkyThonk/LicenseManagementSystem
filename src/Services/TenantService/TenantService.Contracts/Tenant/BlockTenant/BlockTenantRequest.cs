using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Tenant.BlockTenant;

/// <summary>
/// Request to activate or deactivate a government agency
/// </summary>
public record BlockTenantRequest(
    [Required(ErrorMessage = "Tenant ID is required")]
    Guid TenantId,

    bool Activate = false
);

