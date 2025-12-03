using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.Renewals.ProcessRenewal;
using LicenseService.Domain.Renewals;

namespace LicenseService.Application.Renewals.Commands.ProcessRenewal;

/// <summary>
/// Handler for processing a renewal (approve/reject)
/// </summary>
public class ProcessRenewalCommandHandler : ICommandHandler<ProcessRenewalRequest, ProcessRenewalResponse>
{
    private readonly IRenewalRepository _renewalRepository;
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessRenewalCommandHandler(
        IRenewalRepository renewalRepository,
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _renewalRepository = renewalRepository;
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProcessRenewalResponse>> Handle(ProcessRenewalRequest request, CancellationToken ct)
    {
        var renewal = await _renewalRepository.GetByIdAsync(new RenewalId(request.Id), ct);
        if (renewal is null)
        {
            return Result<ProcessRenewalResponse>.Failure(new NotFoundError("Renewal not found"));
        }

        // Start processing if pending
        if (renewal.Status == Domain.Common.Enums.RenewalStatus.Pending)
        {
            renewal.StartProcessing();
        }

        DateTime? newExpiryDate = null;

        if (request.Approve)
        {
            renewal.Approve();

            // Update the license expiry date
            var license = await _licenseRepository.GetByIdAsync(renewal.LicenseId, ct);
            if (license is not null)
            {
                license.Renew(renewal.RenewalDate);
                _licenseRepository.Update(license);
                newExpiryDate = renewal.RenewalDate;
            }

            renewal.Complete();
        }
        else
        {
            renewal.Reject();
        }

        _renewalRepository.Update(renewal);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new ProcessRenewalResponse(
            Id: renewal.Id.Value,
            Status: renewal.Status.ToString(),
            ProcessedAt: renewal.ProcessedAt ?? DateTime.UtcNow,
            NewExpiryDate: newExpiryDate
        );

        return Result<ProcessRenewalResponse>.Success(response);
    }
}
