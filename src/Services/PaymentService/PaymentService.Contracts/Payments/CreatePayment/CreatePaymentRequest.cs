namespace PaymentService.Contracts.Payments.CreatePayment;

/// <summary>
/// Request to create a new payment
/// </summary>
public record CreatePaymentRequest(
    Guid LicenseId,
    Guid ApplicantId,
    decimal Amount,
    string Currency = "INR"
);
