using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.Tenant.GetTenantList;
using TenantService.Contracts.Common.Dto;

namespace TenantService.Application.Tenant.Query.GetTenantList;

/// <summary>
/// Handler for getting paginated list of government agencies (tenants)
/// </summary>
public class GetTenantListQueryHandler : IQueryHandler<GetTenantListRequest, GetTenantListResponse>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantListQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<GetTenantListResponse>> Handle(GetTenantListRequest request, CancellationToken ct)
    {
        var (items, totalCount) = await _tenantRepository.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.SortBy,
            request.SortDescending,
            request.IsActive,
            ct);

        var tenantItems = items.Select(tenant => new TenantListItemDto(
            Id: tenant.Id.Value,
            Name: tenant.Name,
            AgencyCode: tenant.AgencyCode,
            Email: tenant.Email,
            IsActive: tenant.IsActive,
            CreatedAt: tenant.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagination = new PaginationDto(
            request.Page,
            request.PageSize,
            totalCount,
            totalPages,
            request.Page < totalPages,
            request.Page > 1
        );

        return Result<GetTenantListResponse>.Success(new GetTenantListResponse(pagination, tenantItems));
    }
}

