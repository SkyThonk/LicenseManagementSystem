using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.GetUserProfile;

public record GetUserProfileRequest(
    [Required(ErrorMessage = "User ID is required")]
    Guid UserId
);
