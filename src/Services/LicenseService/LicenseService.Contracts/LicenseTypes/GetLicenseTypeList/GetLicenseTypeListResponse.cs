using LicenseService.Contracts.Common;

namespace LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;

/// <summary>
/// DTO for license type list item
/// </summary>
public record LicenseTypeListItemDto(
    Guid Id,
    Guid TenantId,
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
