using Common.Application.Result;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.GetPayment;
using PaymentService.Domain.Payments;

namespace PaymentService.Application.Payments.Queries.GetPayment;

/// <summary>
/// Handler for getting payment details
/// </summary>
public class GetPaymentQueryHandler
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<GetPaymentResponse>> Handle(
        GetPaymentRequest request,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var paymentId = new PaymentId(request.PaymentId);
        var payment = await _paymentRepository.GetByIdAsync(paymentId, tenantId, cancellationToken);

        if (payment is null)
        {
            return Result<GetPaymentResponse>.Failure(
                new NotFoundError($"Payment with ID {request.PaymentId} not found."));
        }

        return Result<GetPaymentResponse>.Success(new GetPaymentResponse(
            PaymentId: payment.Id.Value,
            TenantId: payment.TenantId,
            LicenseId: payment.LicenseId,
            ApplicantId: payment.ApplicantId,
            Amount: payment.Amount,
            Currency: payment.Currency,
            Status: payment.Status.ToString(),
            CreatedAt: payment.CreatedAt,
            PaidAt: payment.PaidAt,
            ReferenceNumber: payment.ReferenceNumber,
            ErrorMessage: payment.ErrorMessage));
    }
}
