using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.GetUserList;
using TenantService.Domain.User;

namespace TenantService.Application.User.Query.GetUserList;

public class GetUserListQueryHandler : IQueryHandler<GetUserListRequest, GetUserListResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserListQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserListResponse>> Handle(GetUserListRequest request, CancellationToken ct)
    {
        // Validate pagination parameters
        if (request.Page < 1 || request.PageSize < 1 || request.PageSize > 100)
        {
            return Result<GetUserListResponse>.Failure(
                new ValidationError("Invalid pagination parameters. Page must be >= 1 and PageSize between 1 and 100"));
        }

        // Parse role if provided
        Role? roleFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!Enum.TryParse<Role>(request.Role, true, out var role))
            {
                return Result<GetUserListResponse>.Failure(
                    new ValidationError($"Invalid role: {request.Role}. Valid roles are: TenantAdmin, Admin, User"));
            }
            roleFilter = role;
        }

        // Get paginated users
        var (users, totalCount) = await _userRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.TenantId,
            roleFilter,
            request.IsActive,
            ct);

        // Map to DTOs
        var userDtos = users.Select(u => new UserListItemDto(
            u.Id.Value,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Role.ToString(),
            u.TenantId.Value,
            u.IsActive,
            u.CreatedAt
        )).ToList();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<GetUserListResponse>.Success(new GetUserListResponse(
            userDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        ));
    }
}
