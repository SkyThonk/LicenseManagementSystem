using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.DeleteUser;

public record DeleteUserRequest(
    [Required(ErrorMessage = "User ID is required")]
    Guid UserId
);
