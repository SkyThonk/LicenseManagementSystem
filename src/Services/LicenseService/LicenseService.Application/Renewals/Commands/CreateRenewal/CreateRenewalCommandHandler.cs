using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Renewals.CreateRenewal;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Renewals;

namespace LicenseService.Application.Renewals.Commands.CreateRenewal;

/// <summary>
/// Handler for creating a new renewal request.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class CreateRenewalCommandHandler : ICommandHandler<CreateRenewalRequest, CreateRenewalResponse>
{
    private readonly IRenewalRepository _renewalRepository;
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRenewalCommandHandler(
        IRenewalRepository renewalRepository,
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _renewalRepository = renewalRepository;
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateRenewalResponse>> Handle(CreateRenewalRequest request, CancellationToken ct)
    {
        // Validate license exists
        var license = await _licenseRepository.GetByIdAsync(new LicenseId(request.LicenseId), ct);
        if (license is null)
        {
            return Result<CreateRenewalResponse>.Failure(new NotFoundError("License not found"));
        }

        // Validate renewal date is in the future
        if (request.RenewalDate <= DateTime.UtcNow)
        {
            return Result<CreateRenewalResponse>.Failure(
                new ValidationError("Renewal date must be in the future"));
        }

        // Create the renewal
        var renewal = Renewal.Create(
            new LicenseId(request.LicenseId),
            request.RenewalDate
        );

        _renewalRepository.Add(renewal);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new CreateRenewalResponse(
            Id: renewal.Id.Value,
            LicenseId: renewal.LicenseId.Value,
            RenewalDate: renewal.RenewalDate,
            Status: renewal.Status.ToString(),
            CreatedAt: renewal.CreatedAt
        );

        return Result<CreateRenewalResponse>.Success(response);
    }
}
