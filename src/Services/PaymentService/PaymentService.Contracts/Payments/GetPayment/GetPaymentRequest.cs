namespace PaymentService.Contracts.Payments.GetPayment;

/// <summary>
/// Request to get payment details
/// </summary>
public record GetPaymentRequest(Guid PaymentId);
