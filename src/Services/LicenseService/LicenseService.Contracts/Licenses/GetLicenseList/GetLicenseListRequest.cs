using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.GetLicenseList;

/// <summary>
/// Request for paginated list of licenses
/// </summary>
public record GetLicenseListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    Guid? TenantId = null,
    Guid? ApplicantId = null,
    string? Status = null,
    string? SortBy = null,
    bool SortDescending = false
);
