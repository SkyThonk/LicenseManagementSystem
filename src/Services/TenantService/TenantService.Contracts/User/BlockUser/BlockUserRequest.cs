using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.BlockUser;

public record BlockUserRequest(
    [Required(ErrorMessage = "User ID is required")]
    Guid UserId
);
