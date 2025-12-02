using Common.Application.Result;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.ProcessPayment;
using PaymentService.Domain.Payments;

namespace PaymentService.Application.Payments.Commands.ProcessPayment;

/// <summary>
/// Handler for processing/completing a payment
/// </summary>
public class ProcessPaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProcessPaymentResponse>> Handle(
        ProcessPaymentRequest request,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
        {
            return Result<ProcessPaymentResponse>.Failure(
                new ValidationError("Reference number is required."));
        }

        // Get payment
        var paymentId = new PaymentId(request.PaymentId);
        var payment = await _paymentRepository.GetByIdAsync(paymentId, tenantId, cancellationToken);

        if (payment is null)
        {
            return Result<ProcessPaymentResponse>.Failure(
                new NotFoundError($"Payment with ID {request.PaymentId} not found."));
        }

        // Check for duplicate reference number
        var existingPayment = await _paymentRepository.GetByReferenceNumberAsync(
            request.ReferenceNumber, cancellationToken);

        if (existingPayment is not null && existingPayment.Id != paymentId)
        {
            return Result<ProcessPaymentResponse>.Failure(
                new ConflictError($"Reference number '{request.ReferenceNumber}' is already used."));
        }

        try
        {
            payment.MarkAsPaid(request.ReferenceNumber);
        }
        catch (InvalidOperationException ex)
        {
            return Result<ProcessPaymentResponse>.Failure(
                new ValidationError(ex.Message));
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProcessPaymentResponse>.Success(new ProcessPaymentResponse(
            PaymentId: payment.Id.Value,
            Status: payment.Status.ToString(),
            PaidAt: payment.PaidAt,
            ReferenceNumber: payment.ReferenceNumber!));
    }
}
