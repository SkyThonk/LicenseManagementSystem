using Common.Domain.Abstractions;
using PaymentService.Domain.Common.Enums;

namespace PaymentService.Domain.Payments;

/// <summary>
/// Represents a payment transaction for license fees.
/// Core entity of the PaymentService.
/// </summary>
public sealed class Payment : Entity<PaymentId>
{
    private Payment(
        PaymentId id,
        Guid tenantId,
        Guid licenseId,
        Guid applicantId,
        decimal amount,
        string currency,
        PaymentStatus status,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        TenantId = tenantId;
        LicenseId = licenseId;
        ApplicantId = applicantId;
        Amount = amount;
        Currency = currency;
        Status = status;
        PaidAt = null;
        ReferenceNumber = null;
    }

    // For EF Core
    private Payment() { }

    /// <summary>
    /// The tenant (government agency) this payment belongs to
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Reference to the license this payment is for
    /// </summary>
    public Guid LicenseId { get; private set; }

    /// <summary>
    /// Reference to the applicant making the payment
    /// </summary>
    public Guid ApplicantId { get; private set; }

    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Currency code (default: INR)
    /// </summary>
    public string Currency { get; private set; } = "INR";

    /// <summary>
    /// Current status of the payment
    /// </summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>
    /// When the payment was completed (null if not paid yet)
    /// </summary>
    public DateTime? PaidAt { get; private set; }

    /// <summary>
    /// External transaction reference number
    /// </summary>
    public string? ReferenceNumber { get; private set; }

    /// <summary>
    /// Error message if payment failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Creates a new payment record
    /// </summary>
    public static Payment Create(
        Guid tenantId,
        Guid licenseId,
        Guid applicantId,
        decimal amount,
        string currency = "INR",
        Guid? createdBy = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        return new Payment(
            PaymentId.New(),
            tenantId,
            licenseId,
            applicantId,
            amount,
            currency.ToUpperInvariant(),
            PaymentStatus.Pending,
            createdBy);
    }

    /// <summary>
    /// Marks the payment as successfully paid
    /// </summary>
    public void MarkAsPaid(string referenceNumber)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark payment as paid when status is {Status}.");

        if (string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException("Reference number is required.", nameof(referenceNumber));

        Status = PaymentStatus.Paid;
        PaidAt = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the payment as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark payment as failed when status is {Status}.");

        Status = PaymentStatus.Failed;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Cancels a pending payment
    /// </summary>
    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel payment when status is {Status}.");

        Status = PaymentStatus.Cancelled;
    }

    /// <summary>
    /// Marks a paid payment as refunded
    /// </summary>
    public void Refund(string refundReferenceNumber)
    {
        if (Status != PaymentStatus.Paid)
            throw new InvalidOperationException("Can only refund paid payments.");

        Status = PaymentStatus.Refunded;
        ReferenceNumber = refundReferenceNumber;
    }

    /// <summary>
    /// Retry a failed payment (reset to pending)
    /// </summary>
    public void Retry()
    {
        if (Status != PaymentStatus.Failed)
            throw new InvalidOperationException("Can only retry failed payments.");

        Status = PaymentStatus.Pending;
        ErrorMessage = null;
    }
}
