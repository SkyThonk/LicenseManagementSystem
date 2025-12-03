using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Common;
using LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;

namespace LicenseService.Application.LicenseTypes.Queries.GetLicenseTypeList;

/// <summary>
/// Handler for getting paginated list of license types
/// </summary>
public class GetLicenseTypeListQueryHandler : IQueryHandler<GetLicenseTypeListRequest, GetLicenseTypeListResponse>
{
    private readonly ILicenseTypeRepository _licenseTypeRepository;

    public GetLicenseTypeListQueryHandler(ILicenseTypeRepository licenseTypeRepository)
    {
        _licenseTypeRepository = licenseTypeRepository;
    }

    public async Task<Result<GetLicenseTypeListResponse>> Handle(GetLicenseTypeListRequest request, CancellationToken ct)
    {
        var (items, totalCount) = await _licenseTypeRepository.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            request.TenantId,
            request.SearchTerm,
            request.SortBy,
            request.SortDescending,
            ct
        );

        var licenseTypeItems = items.Select(lt => new LicenseTypeListItemDto(
            Id: lt.Id.Value,
            TenantId: lt.TenantId,
            Name: lt.Name,
            Description: lt.Description,
            FeeAmount: lt.FeeAmount,
            CreatedAt: lt.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new PaginationDto(
            CurrentPage: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );

        var response = new GetLicenseTypeListResponse(pagination, licenseTypeItems);
        return Result<GetLicenseTypeListResponse>.Success(response);
    }
}
