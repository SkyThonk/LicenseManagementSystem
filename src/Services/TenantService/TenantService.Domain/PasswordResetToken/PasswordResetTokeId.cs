namespace TenantService.Domain.PasswordResetToken;

public record PasswordResetTokenId(Guid Value)
{
    public static implicit operator Guid(PasswordResetTokenId id) => id.Value;
    public static implicit operator PasswordResetTokenId(Guid value) => new(value);
};
