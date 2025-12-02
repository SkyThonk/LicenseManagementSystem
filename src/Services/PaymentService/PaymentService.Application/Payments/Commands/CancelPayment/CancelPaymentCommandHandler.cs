using Common.Application.Result;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.CancelPayment;
using PaymentService.Domain.Payments;

namespace PaymentService.Application.Payments.Commands.CancelPayment;

/// <summary>
/// Handler for cancelling a pending payment
/// </summary>
public class CancelPaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CancelPaymentResponse>> Handle(
        CancelPaymentRequest request,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var paymentId = new PaymentId(request.PaymentId);
        var payment = await _paymentRepository.GetByIdAsync(paymentId, tenantId, cancellationToken);

        if (payment is null)
        {
            return Result<CancelPaymentResponse>.Failure(
                new NotFoundError($"Payment with ID {request.PaymentId} not found."));
        }

        try
        {
            payment.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result<CancelPaymentResponse>.Failure(
                new ValidationError(ex.Message));
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CancelPaymentResponse>.Success(new CancelPaymentResponse(
            PaymentId: payment.Id.Value,
            Status: payment.Status.ToString(),
            UpdatedAt: DateTime.UtcNow));
    }
}
