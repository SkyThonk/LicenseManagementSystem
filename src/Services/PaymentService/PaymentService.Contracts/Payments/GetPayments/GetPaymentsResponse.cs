namespace PaymentService.Contracts.Payments.GetPayments;

/// <summary>
/// Response with list of payments
/// </summary>
public record GetPaymentsResponse(
    IReadOnlyList<PaymentDto> Payments,
    int TotalCount,
    int Page,
    int PageSize
);

public record PaymentDto(
    Guid PaymentId,
    Guid LicenseId,
    Guid ApplicantId,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAt,
    DateTime? PaidAt,
    string? ReferenceNumber
);
