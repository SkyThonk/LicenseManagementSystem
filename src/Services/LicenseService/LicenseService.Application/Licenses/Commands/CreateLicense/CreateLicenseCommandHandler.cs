using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Licenses.CreateLicense;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Application.Licenses.Commands.CreateLicense;

/// <summary>
/// Handler for creating a new license application.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class CreateLicenseCommandHandler : ICommandHandler<CreateLicenseRequest, CreateLicenseResponse>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly ILicenseTypeRepository _licenseTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        ILicenseTypeRepository licenseTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _licenseTypeRepository = licenseTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateLicenseResponse>> Handle(CreateLicenseRequest request, CancellationToken ct)
    {
        // Validate license type exists
        var licenseType = await _licenseTypeRepository.GetByIdAsync(new LicenseTypeId(request.LicenseTypeId), ct);
        if (licenseType is null)
        {
            return Result<CreateLicenseResponse>.Failure(new NotFoundError("License type not found"));
        }

        // Create the license
        var license = License.Create(
            request.ApplicantId,
            new LicenseTypeId(request.LicenseTypeId)
        );

        // Submit the license (changes status from Draft to Submitted)
        license.Submit();

        _licenseRepository.Add(license);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new CreateLicenseResponse(
            Id: license.Id.Value,
            ApplicantId: license.ApplicantId,
            LicenseTypeId: license.LicenseTypeId.Value,
            Status: license.Status.ToString(),
            SubmittedAt: license.SubmittedAt,
            CreatedAt: license.CreatedAt
        );

        return Result<CreateLicenseResponse>.Success(response);
    }
}
