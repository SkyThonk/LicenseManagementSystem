using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.RegisterUser;

public record RegisterUserRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(20, ErrorMessage = "Password cannot exceed 20 characters")]
    string Password,

    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    string? FirstName,

    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    string? LastName,

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(Admin|TenantAdmin|User|Applicant)$", ErrorMessage = "Invalid role")]
    string Role,

    [Required(ErrorMessage = "Tenant ID is required")]
    Guid TenantId
);
