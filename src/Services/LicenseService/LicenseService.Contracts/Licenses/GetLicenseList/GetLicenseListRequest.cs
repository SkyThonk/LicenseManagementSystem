using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.GetLicenseList;

/// <summary>
/// Request for paginated list of licenses.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record GetLicenseListRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    Guid? ApplicantId = null,

    [RegularExpression("^(Pending|Approved|Rejected|Cancelled)?$", ErrorMessage = "Invalid status")]
    string? Status = null,

    [RegularExpression("^(submittedat|approvedat|expirydate|status)?$", ErrorMessage = "Invalid sort field")]
    string? SortBy = null,

    bool SortDescending = false
);
