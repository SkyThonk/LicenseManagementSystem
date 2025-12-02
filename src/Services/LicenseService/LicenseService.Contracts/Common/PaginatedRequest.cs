namespace LicenseService.Contracts.Common;

public record PaginatedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDescending = false
);
