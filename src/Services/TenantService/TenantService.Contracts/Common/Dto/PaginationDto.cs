namespace TenantService.Contracts.Common.Dto;

public record PaginationDto(
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);


