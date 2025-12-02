namespace PaymentService.Contracts.Payments.CancelPayment;

/// <summary>
/// Response after cancelling a payment
/// </summary>
public record CancelPaymentResponse(
    Guid PaymentId,
    string Status,
    DateTime UpdatedAt
);
