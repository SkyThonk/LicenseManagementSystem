namespace PaymentService.Contracts.Payments.ProcessPayment;

/// <summary>
/// Response after processing a payment
/// </summary>
public record ProcessPaymentResponse(
    Guid PaymentId,
    string Status,
    DateTime? PaidAt,
    string ReferenceNumber
);
