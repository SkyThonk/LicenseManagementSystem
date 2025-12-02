using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Authentication;

public record LoginRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MaxLength(20)]
    [MinLength(6)]
    string Password
);

