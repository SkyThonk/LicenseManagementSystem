using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Authentication;

public record ConfirmPasswordResetRequest(
    [Required]
    [MaxLength(100)]
    string Token,

    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    string NewPassword
);
