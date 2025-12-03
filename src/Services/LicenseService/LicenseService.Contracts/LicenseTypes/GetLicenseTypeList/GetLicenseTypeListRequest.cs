using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;

/// <summary>
/// Request for paginated list of license types
/// </summary>
public record GetLicenseTypeListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    Guid? TenantId = null,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false
);
