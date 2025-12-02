using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Tenant.GetTenantList;

/// <summary>
/// Request for paginated list of government agencies
/// </summary>
public record GetTenantListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    string? SearchTerm = null,
    bool? IsActive = null,
    string? SortBy = null,
    bool SortDescending = false
);
