using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.GetRenewalList;

/// <summary>
/// Request for paginated list of renewals.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record GetRenewalListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    Guid? LicenseId = null,
    string? Status = null,
    string? SortBy = null,
    bool SortDescending = false
);
