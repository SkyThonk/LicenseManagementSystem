using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Licenses.GetLicense;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Licenses.Queries.GetLicense;

/// <summary>
/// Handler for getting a license by ID.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetLicenseQueryHandler : IQueryHandler<GetLicenseRequest, LicenseDto>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicenseQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<Result<LicenseDto>> Handle(GetLicenseRequest request, CancellationToken ct)
    {
        var license = await _licenseRepository.GetByIdAsync(new LicenseId(request.Id), ct);
        if (license is null)
        {
            return Result<LicenseDto>.Failure(new NotFoundError("License not found"));
        }

        var dto = new LicenseDto(
            license.Id.Value,
            license.ApplicantId,
            license.LicenseTypeId.Value,
            license.LicenseType?.Name,
            license.Status.ToString(),
            license.SubmittedAt,
            license.ApprovedAt,
            license.ExpiryDate,
            license.CreatedAt,
            license.UpdatedAt
        );

        return Result<LicenseDto>.Success(dto);
    }
}
