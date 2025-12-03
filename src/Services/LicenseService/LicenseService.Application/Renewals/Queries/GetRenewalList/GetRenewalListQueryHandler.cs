using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Common;
using LicenseService.Contracts.Renewals.GetRenewalList;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Renewals.Queries.GetRenewalList;

/// <summary>
/// Handler for getting paginated list of renewals.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetRenewalListQueryHandler : IQueryHandler<GetRenewalListRequest, GetRenewalListResponse>
{
    private readonly IRenewalRepository _renewalRepository;

    public GetRenewalListQueryHandler(IRenewalRepository renewalRepository)
    {
        _renewalRepository = renewalRepository;
    }

    public async Task<Result<GetRenewalListResponse>> Handle(GetRenewalListRequest request, CancellationToken ct)
    {
        LicenseId? licenseId = request.LicenseId.HasValue 
            ? new LicenseId(request.LicenseId.Value) 
            : null;

        var (items, totalCount) = await _renewalRepository.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            licenseId,
            request.Status,
            request.SortBy,
            request.SortDescending,
            ct
        );

        var renewalItems = items.Select(r => new RenewalListItemDto(
            Id: r.Id.Value,
            LicenseId: r.LicenseId.Value,
            RenewalDate: r.RenewalDate,
            Status: r.Status.ToString(),
            ProcessedAt: r.ProcessedAt,
            CreatedAt: r.CreatedAt
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new PaginationDto(
            CurrentPage: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );

        var response = new GetRenewalListResponse(pagination, renewalItems);
        return Result<GetRenewalListResponse>.Success(response);
    }
}
