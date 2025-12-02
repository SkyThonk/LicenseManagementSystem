using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.UnblockUser;
using TenantService.Domain.User;

namespace TenantService.Application.User.Command.UnblockUser;

public class UnblockUserCommandHandler : ICommandHandler<UnblockUserRequest, UnblockUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnblockUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UnblockUserResponse>> Handle(UnblockUserRequest request, CancellationToken ct)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(new UserId(request.UserId), ct);
        if (user is null)
        {
            return Result<UnblockUserResponse>.Failure(
                new NotFoundError("User not found"));
        }

        // Check if already active
        if (user.IsActive)
        {
            return Result<UnblockUserResponse>.Failure(
                new ValidationError("User is already active"));
        }

        // Unblock user
        user.Unblock();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<UnblockUserResponse>.Success(new UnblockUserResponse(
            true,
            "User unblocked successfully"
        ));
    }
}
