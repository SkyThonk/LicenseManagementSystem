using Common.Domain.Abstractions;
using LicenseService.Domain.Common.Enums;
using LicenseService.Domain.LicenseTypes;

namespace LicenseService.Domain.Licenses;

/// <summary>
/// Represents a license application record (core aggregate root).
/// Each tenant has their own isolated database, so TenantId is not stored in the entity.
/// </summary>
public sealed class License : Entity<LicenseId>
{
    private License(
        LicenseId id,
        Guid applicantId,
        LicenseTypeId licenseTypeId,
        LicenseStatus status,
        DateTime submittedAt,
        DateTime? approvedAt,
        DateTime? expiryDate,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        ApplicantId = applicantId;
        LicenseTypeId = licenseTypeId;
        Status = status;
        SubmittedAt = submittedAt;
        ApprovedAt = approvedAt;
        ExpiryDate = expiryDate;
    }

    // For EF Core
    private License() { }

    /// <summary>
    /// The applicant who applied for the license (from IdentityService)
    /// </summary>
    public Guid ApplicantId { get; private set; }

    /// <summary>
    /// The type of license being applied for
    /// </summary>
    public LicenseTypeId LicenseTypeId { get; private set; } = null!;

    /// <summary>
    /// Navigation property to license type
    /// </summary>
    public LicenseType? LicenseType { get; private set; }

    /// <summary>
    /// Current status of the license application
    /// </summary>
    public LicenseStatus Status { get; private set; }

    /// <summary>
    /// When the license application was submitted
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// When the license was approved (null if not yet approved)
    /// </summary>
    public DateTime? ApprovedAt { get; private set; }

    /// <summary>
    /// When the license expires (null if no expiry)
    /// </summary>
    public DateTime? ExpiryDate { get; private set; }

    /// <summary>
    /// Creates a new license application
    /// </summary>
    public static License Create(
        Guid applicantId,
        LicenseTypeId licenseTypeId,
        Guid? createdBy = null)
    {
        var license = new License(
            new LicenseId(Guid.NewGuid()),
            applicantId,
            licenseTypeId,
            LicenseStatus.Draft,
            DateTime.UtcNow,
            null,
            null,
            createdBy
        );

        return license;
    }

    /// <summary>
    /// Submits the license application for review
    /// </summary>
    public void Submit()
    {
        if (Status != LicenseStatus.Draft)
            throw new InvalidOperationException("Only draft licenses can be submitted.");

        Status = LicenseStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the license status
    /// </summary>
    public void UpdateStatus(LicenseStatus newStatus, DateTime? expiryDate = null)
    {
        Status = newStatus;

        if (newStatus == LicenseStatus.Approved)
        {
            ApprovedAt = DateTime.UtcNow;
            if (expiryDate.HasValue)
            {
                ExpiryDate = expiryDate.Value;
            }
        }
    }

    /// <summary>
    /// Approves the license with an optional expiry date
    /// </summary>
    public void Approve(DateTime? expiryDate = null)
    {
        if (Status != LicenseStatus.Submitted && Status != LicenseStatus.UnderReview)
            throw new InvalidOperationException("Only submitted or under-review licenses can be approved.");

        Status = LicenseStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate;
    }

    /// <summary>
    /// Rejects the license application
    /// </summary>
    public void Reject()
    {
        if (Status != LicenseStatus.Submitted && Status != LicenseStatus.UnderReview)
            throw new InvalidOperationException("Only submitted or under-review licenses can be rejected.");

        Status = LicenseStatus.Rejected;
    }

    /// <summary>
    /// Renews the license with a new expiry date
    /// </summary>
    public void Renew(DateTime newExpiryDate)
    {
        if (Status != LicenseStatus.Approved && Status != LicenseStatus.Expired)
            throw new InvalidOperationException("Only approved or expired licenses can be renewed.");

        Status = LicenseStatus.Approved;
        ExpiryDate = newExpiryDate;
    }
}
