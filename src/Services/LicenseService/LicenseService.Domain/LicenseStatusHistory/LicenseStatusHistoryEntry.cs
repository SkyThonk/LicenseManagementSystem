using Common.Domain.Abstractions;
using LicenseService.Domain.Common.Enums;
using LicenseService.Domain.Licenses;

namespace LicenseService.Domain.LicenseStatusHistory;

/// <summary>
/// Represents a status change audit trail for a license.
/// Tracks every status change for compliance and review purposes.
/// </summary>
public sealed class LicenseStatusHistoryEntry : Entity<LicenseStatusHistoryId>
{
    private LicenseStatusHistoryEntry(
        LicenseStatusHistoryId id,
        Guid tenantId,
        LicenseId licenseId,
        LicenseStatus oldStatus,
        LicenseStatus newStatus,
        Guid changedBy,
        DateTime changedAt,
        string? remarks,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        TenantId = tenantId;
        LicenseId = licenseId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedBy = changedBy;
        ChangedAt = changedAt;
        Remarks = remarks;
    }

    // For EF Core
    private LicenseStatusHistoryEntry() { }

    /// <summary>
    /// The tenant (government agency) that owns this record
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// The license this history entry belongs to
    /// </summary>
    public LicenseId LicenseId { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the license
    /// </summary>
    public License? License { get; private set; }

    /// <summary>
    /// The previous status before the change
    /// </summary>
    public LicenseStatus OldStatus { get; private set; }

    /// <summary>
    /// The new status after the change
    /// </summary>
    public LicenseStatus NewStatus { get; private set; }

    /// <summary>
    /// The user who made the status change
    /// </summary>
    public Guid ChangedBy { get; private set; }

    /// <summary>
    /// When the status change occurred
    /// </summary>
    public DateTime ChangedAt { get; private set; }

    /// <summary>
    /// Optional remarks or reason for the status change
    /// </summary>
    public string? Remarks { get; private set; }

    /// <summary>
    /// Creates a new license status history entry
    /// </summary>
    public static LicenseStatusHistoryEntry Create(
        Guid tenantId,
        LicenseId licenseId,
        LicenseStatus oldStatus,
        LicenseStatus newStatus,
        Guid changedBy,
        string? remarks = null,
        Guid? createdBy = null)
    {
        return new LicenseStatusHistoryEntry(
            new LicenseStatusHistoryId(Guid.NewGuid()),
            tenantId,
            licenseId,
            oldStatus,
            newStatus,
            changedBy,
            DateTime.UtcNow,
            remarks,
            createdBy ?? changedBy
        );
    }
}
