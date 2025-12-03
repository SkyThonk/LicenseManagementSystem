using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Renewals.GetRenewal;
using LicenseService.Domain.Renewals;

namespace LicenseService.Application.Renewals.Queries.GetRenewal;

/// <summary>
/// Handler for getting a renewal by ID.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetRenewalQueryHandler : IQueryHandler<GetRenewalRequest, RenewalDto>
{
    private readonly IRenewalRepository _renewalRepository;

    public GetRenewalQueryHandler(IRenewalRepository renewalRepository)
    {
        _renewalRepository = renewalRepository;
    }

    public async Task<Result<RenewalDto>> Handle(GetRenewalRequest request, CancellationToken ct)
    {
        var renewal = await _renewalRepository.GetByIdAsync(new RenewalId(request.Id), ct);
        if (renewal is null)
        {
            return Result<RenewalDto>.Failure(new NotFoundError("Renewal not found"));
        }

        var dto = new RenewalDto(
            Id: renewal.Id.Value,
            LicenseId: renewal.LicenseId.Value,
            RenewalDate: renewal.RenewalDate,
            Status: renewal.Status.ToString(),
            ProcessedAt: renewal.ProcessedAt,
            CreatedAt: renewal.CreatedAt,
            UpdatedAt: renewal.UpdatedAt
        );

        return Result<RenewalDto>.Success(dto);
    }
}
