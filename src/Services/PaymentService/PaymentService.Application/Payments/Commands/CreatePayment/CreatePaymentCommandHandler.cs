using Common.Application.Result;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Contracts.Payments.CreatePayment;
using PaymentService.Domain.Payments;

namespace PaymentService.Application.Payments.Commands.CreatePayment;

/// <summary>
/// Handler for creating a new payment
/// </summary>
public class CreatePaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatePaymentResponse>> Handle(
        CreatePaymentRequest request,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        // Validate request
        if (request.Amount <= 0)
        {
            return Result<CreatePaymentResponse>.Failure(
                new ValidationError("Amount must be greater than zero."));
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            return Result<CreatePaymentResponse>.Failure(
                new ValidationError("Currency is required."));
        }

        // Create payment
        var payment = Payment.Create(
            tenantId: tenantId,
            licenseId: request.LicenseId,
            applicantId: request.ApplicantId,
            amount: request.Amount,
            currency: request.Currency);

        _paymentRepository.Add(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreatePaymentResponse>.Success(new CreatePaymentResponse(
            PaymentId: payment.Id.Value,
            Status: payment.Status.ToString(),
            CreatedAt: payment.CreatedAt));
    }
}
