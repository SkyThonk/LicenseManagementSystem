using TenantService.Domain.PasswordResetToken;
using TenantService.Domain.User;

namespace TenantService.Application.Common.Interfaces.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<PasswordResetToken?> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    void Add(PasswordResetToken token);
    void Update(PasswordResetToken token);
}
