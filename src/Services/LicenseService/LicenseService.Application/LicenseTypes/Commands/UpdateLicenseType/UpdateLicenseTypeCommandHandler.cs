using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseTypes.UpdateLicenseType;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.LicenseTypes.Commands.UpdateLicenseType;

/// <summary>
/// Handler for updating a license type
/// </summary>
public class UpdateLicenseTypeCommandHandler : ICommandHandler<UpdateLicenseTypeRequest, UpdateLicenseTypeResponse>
{
    private readonly ILicenseTypeRepository _licenseTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLicenseTypeCommandHandler(
        ILicenseTypeRepository licenseTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseTypeRepository = licenseTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateLicenseTypeResponse>> Handle(UpdateLicenseTypeRequest request, CancellationToken ct)
    {
        var licenseType = await _licenseTypeRepository.GetByIdAsync(new LicenseTypeId(request.Id), ct);
        if (licenseType is null)
        {
            return Result<UpdateLicenseTypeResponse>.Failure(new NotFoundError("License type not found"));
        }

        // Update the license type
        licenseType.Update(request.Name, request.Description, request.FeeAmount);

        _licenseTypeRepository.Update(licenseType);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new UpdateLicenseTypeResponse(
            Id: licenseType.Id.Value,
            Name: licenseType.Name,
            Description: licenseType.Description,
            FeeAmount: licenseType.FeeAmount,
            UpdatedAt: licenseType.UpdatedAt
        );

        return Result<UpdateLicenseTypeResponse>.Success(response);
    }
}
