namespace PaymentService.Contracts.Payments.CreatePayment;

/// <summary>
/// Response after creating a payment
/// </summary>
public record CreatePaymentResponse(
    Guid PaymentId,
    string Status,
    DateTime CreatedAt
);
