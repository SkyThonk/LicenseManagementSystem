using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.RegisterUser;

public record RegisterUserRequest(
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email,

    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    string Password,

    [MaxLength(100)]
    string? FirstName,

    [MaxLength(100)]
    string? LastName,

    [Required]
    string Role,

    [Required]
    Guid TenantId
);
