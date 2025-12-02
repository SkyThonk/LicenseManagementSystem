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

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<User>()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<User> Users, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? tenantId = null, 
        Role? role = null, 
        bool? isActive = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dataContext.Set<User>().AsQueryable();

        // Apply filters
        if (tenantId.HasValue)
        {
            query = query.Where(u => u.TenantId.Value == tenantId.Value);
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public new void Update(User user)
    {
        base.Update(user);
    }

    public new void Delete(User user)
    {
        base.Delete(user);
    }
}
