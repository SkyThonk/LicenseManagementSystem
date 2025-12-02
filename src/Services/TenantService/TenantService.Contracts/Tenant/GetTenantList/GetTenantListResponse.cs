using TenantService.Contracts.Common.Dto;

namespace TenantService.Contracts.Tenant.GetTenantList;

/// <summary>
/// DTO for tenant list item
/// </summary>
public record TenantListItemDto(
    Guid Id,
    string Name,
    string AgencyCode,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Response for paginated tenant list
/// </summary>
public record GetTenantListResponse(
    PaginationDto Pagination,
    IEnumerable<TenantListItemDto> Items
);
