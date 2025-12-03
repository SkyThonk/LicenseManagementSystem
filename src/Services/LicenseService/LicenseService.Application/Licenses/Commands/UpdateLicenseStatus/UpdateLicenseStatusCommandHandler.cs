using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Licenses.UpdateLicenseStatus;
using LicenseService.Domain.Common.Enums;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.Licenses.Commands.UpdateLicenseStatus;

/// <summary>
/// Handler for updating license status
/// </summary>
public class UpdateLicenseStatusCommandHandler : ICommandHandler<UpdateLicenseStatusRequest, UpdateLicenseStatusResponse>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLicenseStatusCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateLicenseStatusResponse>> Handle(UpdateLicenseStatusRequest request, CancellationToken ct)
    {
        var license = await _licenseRepository.GetByIdAsync(new LicenseId(request.Id), ct);
        if (license is null)
        {
            return Result<UpdateLicenseStatusResponse>.Failure(new NotFoundError("License not found"));
        }

        // Parse the new status
        if (!Enum.TryParse<LicenseStatus>(request.NewStatus, true, out var newStatus))
        {
            return Result<UpdateLicenseStatusResponse>.Failure(new ValidationError("Invalid license status"));
        }

        // Handle specific status transitions
        switch (newStatus)
        {
            case LicenseStatus.Approved:
                license.Approve(request.ExpiryDate);
                break;
            case LicenseStatus.Rejected:
                license.Reject();
                break;
            default:
                license.UpdateStatus(newStatus, request.ExpiryDate);
                break;
        }

        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new UpdateLicenseStatusResponse(
            Id: license.Id.Value,
            Status: license.Status.ToString(),
            ApprovedAt: license.ApprovedAt,
            ExpiryDate: license.ExpiryDate,
            UpdatedAt: license.UpdatedAt
        );

        return Result<UpdateLicenseStatusResponse>.Success(response);
    }
}
