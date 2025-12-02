using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Authentication;

public record RequestPasswordResetRequest(
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email
);
