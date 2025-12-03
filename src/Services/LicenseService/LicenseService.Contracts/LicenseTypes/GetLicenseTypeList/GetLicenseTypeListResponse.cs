using LicenseService.Contracts.Common;

namespace LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;

/// <summary>
/// DTO for license type list item.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseTypeListItemDto(
    Guid Id,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt
);

/// <summary>
/// Response for paginated license type list
/// </summary>
public record GetLicenseTypeListResponse(
    PaginationDto Pagination,
    IEnumerable<LicenseTypeListItemDto> Items
);
