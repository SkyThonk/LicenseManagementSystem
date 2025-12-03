using System.ComponentModel.DataAnnotations;

namespace PaymentService.Contracts.Payments.CancelPayment;

/// <summary>
/// Request to cancel a pending payment
/// </summary>
public record CancelPaymentRequest(
    [Required(ErrorMessage = "Payment ID is required")]
    Guid PaymentId
);
