using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Common;
using LicenseService.Contracts.Licenses.GetLicenseList;

namespace LicenseService.Application.Licenses.Queries.GetLicenseList;

/// <summary>
/// Handler for getting paginated list of licenses.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetLicenseListQueryHandler : IQueryHandler<GetLicenseListRequest, GetLicenseListResponse>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicenseListQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<Result<GetLicenseListResponse>> Handle(GetLicenseListRequest request, CancellationToken ct)
    {
        var (items, totalCount) = await _licenseRepository.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            request.ApplicantId,
            request.Status,
            request.SortBy,
            request.SortDescending,
            ct
        );

        var licenseItems = items.Select(l => new LicenseListItemDto(
            l.Id.Value,
            l.ApplicantId,
            l.LicenseTypeId.Value,
            l.LicenseType?.Name,
            l.Status.ToString(),
            l.SubmittedAt,
            l.ExpiryDate,
            l.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new PaginationDto(
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        );

        var response = new GetLicenseListResponse(pagination, licenseItems);
        return Result<GetLicenseListResponse>.Success(response);
    }
}
