using Common.Application.Result;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.GetPayments;
using PaymentService.Domain.Common.Enums;

namespace PaymentService.Application.Payments.Queries.GetPayments;

/// <summary>
/// Handler for getting list of payments
/// </summary>
public class GetPaymentsQueryHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<GetPaymentsResponse>> Handle(
        GetPaymentsRequest request,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Payments.Payment> payments;

        // Filter by license or applicant if specified
        if (request.LicenseId.HasValue)
        {
            payments = await _paymentRepository.GetByLicenseIdAsync(request.LicenseId.Value, cancellationToken);
            // Filter by tenant
            payments = payments.Where(p => p.TenantId == tenantId).ToList();
        }
        else if (request.ApplicantId.HasValue)
        {
            payments = await _paymentRepository.GetByApplicantIdAsync(request.ApplicantId.Value, tenantId, cancellationToken);
        }
        else
        {
            payments = await _paymentRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        }

        // Filter by status if specified
        if (!string.IsNullOrWhiteSpace(request.Status) && 
            Enum.TryParse<PaymentStatus>(request.Status, true, out var status))
        {
            payments = payments.Where(p => p.Status == status).ToList();
        }

        var totalCount = payments.Count;

        // Apply pagination
        var pagedPayments = payments
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PaymentDto(
                PaymentId: p.Id.Value,
                LicenseId: p.LicenseId,
                ApplicantId: p.ApplicantId,
                Amount: p.Amount,
                Currency: p.Currency,
                Status: p.Status.ToString(),
                CreatedAt: p.CreatedAt,
                PaidAt: p.PaidAt,
                ReferenceNumber: p.ReferenceNumber))
            .ToList();

        return Result<GetPaymentsResponse>.Success(new GetPaymentsResponse(
            Payments: pagedPayments,
            TotalCount: totalCount,
            Page: request.Page,
            PageSize: request.PageSize));
    }
}
