using Common.Domain.Abstractions;

namespace LicenseService.Domain.LicenseTypes;

/// <summary>
/// Represents a license type/category.
/// Each tenant has their own isolated database, so TenantId is not stored in the entity.
/// </summary>
public sealed class LicenseType : Entity<LicenseTypeId>
{
    private LicenseType(
        LicenseTypeId id,
        string name,
        string? description,
        decimal feeAmount,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        Name = name;
        Description = description;
        FeeAmount = feeAmount;
    }

    // For EF Core
    private LicenseType() { }

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
