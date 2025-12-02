namespace PaymentService.Domain.Common.Enums;

/// <summary>
/// Status of a payment transaction
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment has been initiated but not yet completed
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been successfully processed
    /// </summary>
    Paid,

    /// <summary>
    /// Payment processing failed
    /// </summary>
    Failed,

    /// <summary>
    /// Payment was cancelled before processing
    /// </summary>
    Cancelled,

    /// <summary>
    /// Payment has been refunded
    /// </summary>
    Refunded
}
