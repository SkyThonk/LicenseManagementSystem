using Common.Domain.Abstractions;
using TenantService.Domain.User;

namespace TenantService.Domain.PasswordResetToken;

public sealed class PasswordResetToken : Entity<PasswordResetTokenId>
{
    private PasswordResetToken(
        PasswordResetTokenId id,
        UserId userId,
        string token,
        DateTime expiresAt,
        bool isUsed
    ) : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsUsed = isUsed;
        CreatedAt = DateTime.UtcNow;
    }

    // For EF Core
    private PasswordResetToken() { }

    public UserId UserId { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public static PasswordResetToken Create(UserId userId, int expirationHours = 1)
    {
        var token = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        return new PasswordResetToken(
            new PasswordResetTokenId(Guid.NewGuid()),
            userId,
            token,
            expiresAt,
            false);
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }

    private static string GenerateSecureToken()
    {
        // Generate a cryptographically secure random token
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}
