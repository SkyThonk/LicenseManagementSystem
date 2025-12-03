namespace LicenseService.Contracts.Common;

/// <summary>
/// Pagination metadata DTO
/// </summary>
public record PaginationDto(
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages
);
