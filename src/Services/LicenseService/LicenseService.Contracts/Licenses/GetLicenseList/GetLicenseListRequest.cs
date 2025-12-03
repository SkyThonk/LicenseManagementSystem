using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.GetLicenseList;

/// <summary>
/// Request for paginated list of licenses.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record GetLicenseListRequest(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    Guid? ApplicantId = null,
    string? Status = null,
    string? SortBy = null,
    bool SortDescending = false
);
