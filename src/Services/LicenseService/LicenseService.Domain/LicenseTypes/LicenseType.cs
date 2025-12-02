using Common.Domain.Abstractions;

namespace LicenseService.Domain.LicenseTypes;

/// <summary>
/// Represents a license type/category that can be defined per tenant.
/// Each tenant can define their own license categories (e.g., Medical License, Driver License, etc.)
/// </summary>
public sealed class LicenseType : Entity<LicenseTypeId>
{
    private LicenseType(
        LicenseTypeId id,
        Guid tenantId,
        string name,
        string? description,
        decimal feeAmount,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        TenantId = tenantId;
        Name = name;
        Description = description;
        FeeAmount = feeAmount;
    }

    // For EF Core
    private LicenseType() { }

    /// <summary>
    /// The tenant (government agency) that owns this license type
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Name of the license type (e.g., "Medical License", "Driver License")
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Description of the license type
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Fee amount required for this license type
    /// </summary>
    public decimal FeeAmount { get; private set; }

    /// <summary>
    /// Creates a new license type
    /// </summary>
    public static LicenseType Create(
        Guid tenantId,
        string name,
        string? description,
        decimal feeAmount,
        Guid? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (feeAmount < 0)
            throw new ArgumentException("Fee amount cannot be negative.", nameof(feeAmount));

        return new LicenseType(
            new LicenseTypeId(Guid.NewGuid()),
            tenantId,
            name,
            description,
            feeAmount,
            createdBy
        );
    }

    /// <summary>
    /// Updates the license type details
    /// </summary>
    public void Update(string name, string? description, decimal feeAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (feeAmount < 0)
            throw new ArgumentException("Fee amount cannot be negative.", nameof(feeAmount));

        Name = name;
        Description = description;
        FeeAmount = feeAmount;
    }
}
