using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseTypes.CreateLicenseType;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.LicenseTypes.Commands.CreateLicenseType;

/// <summary>
/// Handler for creating a new license type
/// </summary>
public class CreateLicenseTypeCommandHandler : ICommandHandler<CreateLicenseTypeRequest, CreateLicenseTypeResponse>
{
    private readonly ILicenseTypeRepository _licenseTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLicenseTypeCommandHandler(
        ILicenseTypeRepository licenseTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseTypeRepository = licenseTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateLicenseTypeResponse>> Handle(CreateLicenseTypeRequest request, CancellationToken ct)
    {
        // Check if license type with same name already exists for this tenant
        if (await _licenseTypeRepository.ExistsByNameAsync(request.TenantId, request.Name, ct))
        {
            return Result<CreateLicenseTypeResponse>.Failure(
                new ValidationError("License type with this name already exists for this tenant"));
        }

        // Create the license type
        var licenseType = LicenseType.Create(
            request.TenantId,
            request.Name,
            request.Description,
            request.FeeAmount
        );

        _licenseTypeRepository.Add(licenseType);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new CreateLicenseTypeResponse(
            Id: licenseType.Id.Value,
            TenantId: licenseType.TenantId,
            Name: licenseType.Name,
            Description: licenseType.Description,
            FeeAmount: licenseType.FeeAmount,
            CreatedAt: licenseType.CreatedAt
        );

        return Result<CreateLicenseTypeResponse>.Success(response);
    }
}
