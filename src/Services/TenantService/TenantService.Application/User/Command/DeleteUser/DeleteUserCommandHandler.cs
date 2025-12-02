using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.User.DeleteUser;
using TenantService.Domain.User;

namespace TenantService.Application.User.Command.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserRequest, DeleteUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteUserResponse>> Handle(DeleteUserRequest request, CancellationToken ct)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(new UserId(request.UserId), ct);
        if (user is null)
        {
            return Result<DeleteUserResponse>.Failure(
                new NotFoundError("User not found"));
        }

        // Soft delete (mark as deleted)
        _userRepository.Delete(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<DeleteUserResponse>.Success(new DeleteUserResponse(
            true,
            "User deleted successfully"
        ));
    }
}
