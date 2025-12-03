using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.GetRenewalList;

/// <summary>
/// Request for paginated list of renewals
/// </summary>
public record GetRenewalListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    Guid? TenantId = null,
    Guid? LicenseId = null,
    string? Status = null,
    string? SortBy = null,
    bool SortDescending = false
);
