namespace PaymentService.Contracts.Payments.GetPayment;

/// <summary>
/// Response with payment details
/// </summary>
public record GetPaymentResponse(
    Guid PaymentId,
    Guid TenantId,
    Guid LicenseId,
    Guid ApplicantId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAt,
    DateTime? PaidAt,
    string? ReferenceNumber,
    string? ErrorMessage
);
