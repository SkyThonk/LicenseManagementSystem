using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.UnblockUser;

public record UnblockUserRequest(
    [Required(ErrorMessage = "User ID is required")]
    Guid UserId
);
