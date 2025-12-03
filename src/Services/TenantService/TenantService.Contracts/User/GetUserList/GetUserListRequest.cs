using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.User.GetUserList;

public record GetUserListRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    Guid? TenantId = null,

    [RegularExpression("^(Admin|TenantAdmin|User|Applicant)?$", ErrorMessage = "Invalid role")]
    string? Role = null,

    bool? IsActive = null
);
