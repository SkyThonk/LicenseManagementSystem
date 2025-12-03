using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.GetRenewalList;

/// <summary>
/// Request for paginated list of renewals.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record GetRenewalListRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    Guid? LicenseId = null,

    [RegularExpression("^(Pending|Approved|Rejected)?$", ErrorMessage = "Invalid status")]
    string? Status = null,

    string? SortBy = null,

    bool SortDescending = false
);
