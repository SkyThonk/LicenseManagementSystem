using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.BlockUser;
using TenantService.Domain.User;

namespace TenantService.Application.User.Command.BlockUser;

public class BlockUserCommandHandler : ICommandHandler<BlockUserRequest, BlockUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BlockUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BlockUserResponse>> Handle(BlockUserRequest request, CancellationToken ct)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(new UserId(request.UserId), ct);
        if (user is null)
        {
            return Result<BlockUserResponse>.Failure(
                new NotFoundError("User not found"));
        }

        // Check if already blocked
        if (!user.IsActive)
        {
            return Result<BlockUserResponse>.Failure(
                new ValidationError("User is already blocked"));
        }

        // Block user
        user.Block();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<BlockUserResponse>.Success(new BlockUserResponse(
            true,
            "User blocked successfully"
        ));
    }
}
