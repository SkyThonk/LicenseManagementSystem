using Microsoft.EntityFrameworkCore;
using TenantService.Persistence.Common.Abstractions;
using TenantService.Persistence.Data;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Domain.User;

namespace TenantService.Persistence.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }
}
