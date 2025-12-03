using System.ComponentModel.DataAnnotations;

namespace PaymentService.Contracts.Payments.GetPayments;

/// <summary>
/// Request to get payments list
/// </summary>
public record GetPaymentsRequest(
    Guid? LicenseId = null,

    Guid? ApplicantId = null,

    [RegularExpression("^(Pending|Completed|Failed|Cancelled)?$", ErrorMessage = "Invalid status")]
    string? Status = null,

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 20
);
