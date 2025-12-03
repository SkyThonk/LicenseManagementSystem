using LicenseService.Contracts.Common;

namespace LicenseService.Contracts.Renewals.GetRenewalList;

/// <summary>
/// DTO for renewal list item
/// </summary>
public record RenewalListItemDto(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    DateTime RenewalDate,
    string Status,
    DateTime? ProcessedAt,
    DateTime CreatedAt
);

/// <summary>
/// Response for paginated renewal list
/// </summary>
public record GetRenewalListResponse(
    PaginationDto Pagination,
    IEnumerable<RenewalListItemDto> Items
);
