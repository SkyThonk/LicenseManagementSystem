using Common.Domain.Abstractions;
using Common.Domain.Events;
using TenantService.Domain.Common.Enum;
using TenantService.Domain.Common.ValueObjects;

namespace TenantService.Domain.Tenant;

/// <summary>
/// Represents a Government Agency (tenant) that manages professional licenses.
/// Each agency operates independently with its own set of licenses (multi-tenancy isolation).
/// </summary>
public sealed class Tenant : Entity<TenantId>
{
    private Tenant(
        TenantId id,
        string name,
        string agencyCode,
        string email,
        string? description,
        string? logo,
        bool isActive,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        Name = name;
        AgencyCode = agencyCode;
        Email = email;
        Description = description;
        Logo = logo;
        IsActive = isActive;
    }

    // For EF Core
    private Tenant() { }

    /// <summary>
    /// Agency name (e.g., "Texas Medical Board")
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Unique agency code for identification
    /// </summary>
    public string AgencyCode { get; private set; } = null!;

    /// <summary>
    /// Agency description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Agency address
    /// </summary>
    public Address Address { get; private set; } = null!;

    /// <summary>
    /// Agency phone number
    /// </summary>
    public PhoneNumber Phone { get; private set; } = null!;

    /// <summary>
    /// Primary contact email
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Agency logo URL
    /// </summary>
    public string? Logo { get; private set; }

    /// <summary>
    /// Whether the agency is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    public static Tenant Create(
        string name,
        string agencyCode,
        Address address,
        PhoneNumber phone,
        string email,
        string? description = null,
        string? logo = null,
        Guid? createdBy = null)
    {
        var tenant = new Tenant(
            new TenantId(Guid.NewGuid()),
            name,
            agencyCode,
            email,
            description,
            logo,
            true,
            createdBy)
        {
            Address = address,
            Phone = phone
        };

        // Raise domain event for tenant creation
        tenant.AddDomainEvent(new TenantCreatedEvent(
            TenantId: tenant.Id.Value,
            Name: tenant.Name,
            AgencyCode: tenant.AgencyCode,
            Email: tenant.Email,
            CreatedAt: tenant.CreatedAt
        ));

        return tenant;
    }

    public void Update(
        string? name = null,
        string? description = null,
        Address? address = null,
        PhoneNumber? phone = null,
        string? email = null,
        string? logo = null,
        Guid? updatedBy = null)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (description is not null) Description = description;
        if (address is not null) Address = address;
        if (phone is not null) Phone = phone;
        if (!string.IsNullOrWhiteSpace(email)) Email = email;
        if (logo is not null) Logo = logo;
        SetUpdatedBy(updatedBy);
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event for tenant update
        AddDomainEvent(new TenantUpdatedEvent(
            TenantId: Id.Value,
            Name: Name,
            AgencyCode: AgencyCode,
            Email: Email,
            IsActive: IsActive,
            UpdatedAt: UpdatedAt
        ));
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        SetUpdatedBy(updatedBy);
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event for tenant activation
        AddDomainEvent(new TenantUpdatedEvent(
            TenantId: Id.Value,
            Name: Name,
            AgencyCode: AgencyCode,
            Email: Email,
            IsActive: IsActive,
            UpdatedAt: UpdatedAt
        ));
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        SetUpdatedBy(updatedBy);
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event for tenant deactivation
        AddDomainEvent(new TenantUpdatedEvent(
            TenantId: Id.Value,
            Name: Name,
            AgencyCode: AgencyCode,
            Email: Email,
            IsActive: IsActive,
            UpdatedAt: UpdatedAt
        ));
    }

    public void Delete(Guid? updatedBy = null)
    {
        IsDeleted = true;
        SetUpdatedBy(updatedBy);
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event for tenant deletion
        AddDomainEvent(new TenantDeletedEvent(
            TenantId: Id.Value,
            DeletedAt: UpdatedAt
        ));
    }
}

