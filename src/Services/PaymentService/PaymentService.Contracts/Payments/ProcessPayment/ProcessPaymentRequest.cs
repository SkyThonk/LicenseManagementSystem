using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentService.Contracts.Payments.ProcessPayment;

/// <summary>
/// Request to process/complete a payment
/// </summary>
public record ProcessPaymentRequest(
    [property: JsonIgnore]
    Guid PaymentId,

    [Required(ErrorMessage = "Reference number is required")]
    [MaxLength(100, ErrorMessage = "Reference number cannot exceed 100 characters")]
    string ReferenceNumber
);
