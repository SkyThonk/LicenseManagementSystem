namespace PaymentService.Contracts.Payments.GetPayments;

/// <summary>
/// Request to get payments list
/// </summary>
public record GetPaymentsRequest(
    Guid? LicenseId = null,
    Guid? ApplicantId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
);
