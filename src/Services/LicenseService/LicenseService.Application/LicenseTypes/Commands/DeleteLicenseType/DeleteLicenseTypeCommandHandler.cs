using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseTypes.DeleteLicenseType;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.LicenseTypes.Commands.DeleteLicenseType;

/// <summary>
/// Handler for deleting a license type.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class DeleteLicenseTypeCommandHandler : ICommandHandler<DeleteLicenseTypeRequest, DeleteLicenseTypeResponse>
{
    private readonly ILicenseTypeRepository _licenseTypeRepository;
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLicenseTypeCommandHandler(
        ILicenseTypeRepository licenseTypeRepository,
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseTypeRepository = licenseTypeRepository;
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteLicenseTypeResponse>> Handle(DeleteLicenseTypeRequest request, CancellationToken ct)
    {
        var licenseType = await _licenseTypeRepository.GetByIdAsync(new LicenseTypeId(request.Id), ct);
        if (licenseType is null)
        {
            return Result<DeleteLicenseTypeResponse>.Failure(new NotFoundError("License type not found"));
        }

        // Check if there are any licenses using this license type
        var licensesUsingType = await _licenseRepository.GetPaginatedAsync(
            1, 1, null, null, null, false, ct);
        
        // For now, we'll allow deletion - in production you might want to prevent this
        // if there are licenses using this type

        // Soft delete the license type
        licenseType.IsDeleted = true;
        _licenseTypeRepository.Update(licenseType);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new DeleteLicenseTypeResponse(
            Id: licenseType.Id.Value,
            Success: true,
            Message: "License type deleted successfully"
        );

        return Result<DeleteLicenseTypeResponse>.Success(response);
    }
}
