using System.ComponentModel.DataAnnotations;

namespace PaymentService.Contracts.Payments.GetPayment;

/// <summary>
/// Request to get payment details
/// </summary>
public record GetPaymentRequest(
    [Required(ErrorMessage = "Payment ID is required")]
    Guid PaymentId
);
