namespace PaymentService.Contracts.Payments.CancelPayment;

/// <summary>
/// Request to cancel a pending payment
/// </summary>
public record CancelPaymentRequest(Guid PaymentId);
