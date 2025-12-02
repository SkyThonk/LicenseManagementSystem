using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record TokenDetails(string Token, string RefreshToken)
{
    public static TokenDetails Create(string token, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token cannot be empty");
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new DomainException("Refresh token cannot be empty");

        return new TokenDetails(token, refreshToken);
    }
}

