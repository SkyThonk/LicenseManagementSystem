using System.ComponentModel.DataAnnotations;

namespace PaymentService.Contracts.Payments.CreatePayment;

/// <summary>
/// Request to create a new payment
/// </summary>
public record CreatePaymentRequest(
    [Required(ErrorMessage = "License ID is required")]
    Guid LicenseId,

    [Required(ErrorMessage = "Applicant ID is required")]
    Guid ApplicantId,

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    decimal Amount,

    [Required(ErrorMessage = "Currency is required")]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code")]
    string Currency = "INR"
);
