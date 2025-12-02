using Common.Domain.Abstractions;
using TenantService.Domain.Tenant;

namespace TenantService.Domain.User;

public class User : Entity<UserId>
{
    private User(
        UserId id,
        string email,
        string passwordHash,
        Role role,
        TenantId tenantId,
        bool isActive
    ) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        TenantId = tenantId;
        IsActive = isActive;
    }

    // For EF Core
    private User() { }

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public static User Create(
        string email,
        string passwordHash,
        Role role,
        TenantId tenantId)
    {
        return new User(
            new UserId(Guid.NewGuid()),
            email,
            passwordHash,
            role,
            tenantId,
            true);
    }
}

public record UserId(Guid Value);
