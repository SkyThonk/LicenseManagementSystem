using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Tenant.GetTenantProfile;

public record GetTenantProfileRequest(
    [Required(ErrorMessage = "Tenant ID is required")]
    Guid TenantId
);

