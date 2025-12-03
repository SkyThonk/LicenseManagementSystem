using Common.Domain.Abstractions;
using LicenseService.Domain.Common.Enums;
using LicenseService.Domain.Licenses;

namespace LicenseService.Domain.Renewals;

/// <summary>
/// Represents a license renewal request.
/// Supports background job processing and expiry logic.
/// Each tenant has their own isolated database, so TenantId is not stored in the entity.
/// </summary>
public sealed class Renewal : Entity<RenewalId>
{
    private Renewal(
        RenewalId id,
        LicenseId licenseId,
        DateTime renewalDate,
        RenewalStatus status,
        DateTime? processedAt,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        LicenseId = licenseId;
        RenewalDate = renewalDate;
        Status = status;
        ProcessedAt = processedAt;
    }

    // For EF Core
    private Renewal() { }

    /// <summary>
    /// The license being renewed
    /// </summary>
    public LicenseId LicenseId { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the license
    /// </summary>
    public License? License { get; private set; }

    /// <summary>
    /// The target renewal date (new expiry date)
    /// </summary>
    public DateTime RenewalDate { get; private set; }

    /// <summary>
    /// Current status of the renewal request
    /// </summary>
    public RenewalStatus Status { get; private set; }

    /// <summary>
    /// When the renewal was processed (null if not yet processed)
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Creates a new renewal request
    /// </summary>
    public static Renewal Create(
        LicenseId licenseId,
        DateTime renewalDate,
        Guid? createdBy = null)
    {
        return new Renewal(
            new RenewalId(Guid.NewGuid()),
            licenseId,
            renewalDate,
            RenewalStatus.Pending,
            null,
            createdBy
        );
    }

    /// <summary>
    /// Marks the renewal as being processed
    /// </summary>
    public void StartProcessing()
    {
        if (Status != RenewalStatus.Pending)
            throw new InvalidOperationException("Only pending renewals can be processed.");

        Status = RenewalStatus.Processing;
    }

    /// <summary>
    /// Approves the renewal
    /// </summary>
    public void Approve()
    {
        if (Status != RenewalStatus.Processing)
            throw new InvalidOperationException("Only processing renewals can be approved.");

        Status = RenewalStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejects the renewal
    /// </summary>
    public void Reject()
    {
        if (Status != RenewalStatus.Processing)
            throw new InvalidOperationException("Only processing renewals can be rejected.");

        Status = RenewalStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the renewal as completed
    /// </summary>
    public void Complete()
    {
        if (Status != RenewalStatus.Approved)
            throw new InvalidOperationException("Only approved renewals can be completed.");

        Status = RenewalStatus.Completed;
    }

    /// <summary>
    /// Marks the renewal as failed
    /// </summary>
    public void Fail()
    {
        Status = RenewalStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}
