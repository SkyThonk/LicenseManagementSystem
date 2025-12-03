using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;

/// <summary>
/// Request for paginated list of license types.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record GetLicenseTypeListRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    string? SearchTerm = null,

    string? SortBy = null,

    bool SortDescending = false
);
