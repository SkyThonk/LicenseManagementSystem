using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.UpdateUser;
using TenantService.Domain.User;

namespace TenantService.Application.User.Command.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserRequest, UpdateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserRequest request, CancellationToken ct)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(new UserId(request.Id), ct);
        if (user is null)
        {
            return Result<UpdateUserResponse>.Failure(
                new NotFoundError("User not found"));
        }

        // Update email if provided
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existingUser is not null)
            {
                return Result<UpdateUserResponse>.Failure(
                    new ValidationError("A user with this email already exists"));
            }
            user.UpdateEmail(request.Email);
        }

        // Update name if provided
        if (request.FirstName is not null || request.LastName is not null)
        {
            user.UpdateName(
                request.FirstName ?? user.FirstName,
                request.LastName ?? user.LastName
            );
        }

        // Update role if provided
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!Enum.TryParse<Role>(request.Role, true, out var role))
            {
                return Result<UpdateUserResponse>.Failure(
                    new ValidationError($"Invalid role: {request.Role}. Valid roles are: TenantAdmin, Admin, User"));
            }
            user.UpdateRole(role);
        }

        // Save changes
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<UpdateUserResponse>.Success(new UpdateUserResponse(
            true,
            "User updated successfully"
        ));
    }
}
