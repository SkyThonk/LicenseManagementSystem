using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.GetUserProfile;
using TenantService.Domain.User;

namespace TenantService.Application.User.Query.GetUserProfile;

public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileRequest, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileRequest request, CancellationToken ct)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(new UserId(request.UserId), ct);
        if (user is null)
        {
            return Result<UserProfileDto>.Failure(
                new NotFoundError("User not found"));
        }

        // Get tenant for tenant name
        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, ct);
        var tenantName = tenant?.Name ?? "Unknown";

        return Result<UserProfileDto>.Success(new UserProfileDto(
            user.Id.Value,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            user.TenantId.Value,
            tenantName,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        ));
    }
}
