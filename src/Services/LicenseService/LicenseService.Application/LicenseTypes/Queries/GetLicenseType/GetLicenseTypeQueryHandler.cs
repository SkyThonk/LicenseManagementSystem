using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseTypes.GetLicenseType;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.LicenseTypes.Queries.GetLicenseType;

/// <summary>
/// Handler for getting a license type by ID.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetLicenseTypeQueryHandler : IQueryHandler<GetLicenseTypeRequest, LicenseTypeDto>
{
    private readonly ILicenseTypeRepository _licenseTypeRepository;

    public GetLicenseTypeQueryHandler(ILicenseTypeRepository licenseTypeRepository)
    {
        _licenseTypeRepository = licenseTypeRepository;
    }

    public async Task<Result<LicenseTypeDto>> Handle(GetLicenseTypeRequest request, CancellationToken ct)
    {
        var licenseType = await _licenseTypeRepository.GetByIdAsync(new LicenseTypeId(request.Id), ct);
        if (licenseType is null)
        {
            return Result<LicenseTypeDto>.Failure(new NotFoundError("License type not found"));
        }

        var dto = new LicenseTypeDto(
            Id: licenseType.Id.Value,
            Name: licenseType.Name,
            Description: licenseType.Description,
            FeeAmount: licenseType.FeeAmount,
            CreatedAt: licenseType.CreatedAt,
            UpdatedAt: licenseType.UpdatedAt
        );

        return Result<LicenseTypeDto>.Success(dto);
    }
}
