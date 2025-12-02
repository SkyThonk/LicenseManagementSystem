using Common.Domain.Abstractions;
using TenantService.Domain.Tenant;

namespace TenantService.Domain.User;

public class User : Entity<UserId>
{
    private User(
        UserId id,
        string email,
        string passwordHash,
        string? firstName,
        string? lastName,
        Role role,
        TenantId tenantId,
        bool isActive
    ) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        TenantId = tenantId;
        IsActive = isActive;
    }

    // For EF Core
    private User() { }

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public Role Role { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public static User Create(
        string email,
        string passwordHash,
        string? firstName,
        string? lastName,
        Role role,
        TenantId tenantId)
    {
        return new User(
            new UserId(Guid.NewGuid()),
            email,
            passwordHash,
            firstName,
            lastName,
            role,
            tenantId,
            true);
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(Role role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unblock()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
