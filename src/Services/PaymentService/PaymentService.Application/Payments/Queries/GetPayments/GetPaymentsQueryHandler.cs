using Common.Application.Result;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.GetPayments;

namespace PaymentService.Application.Payments.Queries.GetPayments;

/// <summary>
/// Handler for getting paginated list of payments.
/// Pagination and filtering happens at the SQL level for efficiency.
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
        // All filtering and pagination happens at SQL level
        var (payments, totalCount) = await _paymentRepository.GetPaginatedAsync(
            tenantId,
            request.Page,
            request.PageSize,
            request.LicenseId,
            request.ApplicantId,
            request.Status,
            cancellationToken);

        var paymentDtos = payments
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
            Payments: paymentDtos,
            TotalCount: totalCount,
            Page: request.Page,
            PageSize: request.PageSize));
    }
}
