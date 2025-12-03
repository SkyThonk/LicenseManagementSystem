using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TenantService.Contracts.User.UpdateUser;

public record UpdateUserRequest(
    [property: JsonIgnore]
    Guid Id,

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    string? Email,

    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    string? FirstName,

    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    string? LastName,

    [RegularExpression("^(Admin|TenantAdmin|User|Applicant)$", ErrorMessage = "Invalid role")]
    string? Role
);
