using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.UpdateUser;

public record UpdateUserRequest(
    Guid Id,

    [EmailAddress]
    [MaxLength(255)]
    string? Email,

    [MaxLength(100)]
    string? FirstName,

    [MaxLength(100)]
    string? LastName,

    string? Role
);
