using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Tenant.GetTenantList;

/// <summary>
/// Request for paginated list of government agencies
/// </summary>
public record GetTenantListRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    string? SearchTerm = null,

    bool? IsActive = null,

    string? SortBy = null,

    bool SortDescending = false
);
