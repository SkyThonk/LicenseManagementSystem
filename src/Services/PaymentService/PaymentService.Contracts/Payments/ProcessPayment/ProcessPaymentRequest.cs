namespace PaymentService.Contracts.Payments.ProcessPayment;

/// <summary>
/// Request to process/complete a payment
/// </summary>
public record ProcessPaymentRequest(
    Guid PaymentId,
    string ReferenceNumber
);
