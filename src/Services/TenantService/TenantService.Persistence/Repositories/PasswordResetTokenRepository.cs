using Microsoft.EntityFrameworkCore;
using TenantService.Persistence.Common.Abstractions;
using TenantService.Persistence.Data;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Domain.User;
using TenantService.Domain.PasswordResetToken;

namespace TenantService.Persistence.Repositories;

internal sealed class PasswordResetTokenRepository : Repository<PasswordResetToken, PasswordResetTokenId>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<PasswordResetToken>()
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public async Task<PasswordResetToken?> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<PasswordResetToken>()
            .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public new void Update(PasswordResetToken token)
    {
        base.Update(token);
    }
}
