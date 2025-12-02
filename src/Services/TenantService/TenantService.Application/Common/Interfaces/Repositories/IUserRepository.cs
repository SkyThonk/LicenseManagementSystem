using TenantService.Domain.User;
using UserR = TenantService.Domain.User;

namespace TenantService.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserR.User?> GetByIdAsync(UserR.UserId id, CancellationToken cancellationToken = default);
    Task<UserR.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<UserR.User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(List<UserR.User> Users, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? tenantId = null, 
        Role? role = null, 
        bool? isActive = null, 
        CancellationToken cancellationToken = default);
    void Add(UserR.User user);
    void Update(UserR.User user);
    void Delete(UserR.User user);
}
