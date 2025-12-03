using LicenseService.Contracts.Common;

namespace LicenseService.Contracts.Licenses.GetLicenseList;

/// <summary>
/// DTO for license list item.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseListItemDto(
    Guid Id,
    Guid ApplicantId,
    Guid LicenseTypeId,
    string? LicenseTypeName,
    string Status,
    DateTime SubmittedAt,
    DateTime? ExpiryDate,
    DateTime CreatedAt
);

/// <summary>
/// Response for paginated license list
/// </summary>
public record GetLicenseListResponse(
    PaginationDto Pagination,
    IEnumerable<LicenseListItemDto> Items
);
