namespace LicenseService.Contracts.Common;

public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
