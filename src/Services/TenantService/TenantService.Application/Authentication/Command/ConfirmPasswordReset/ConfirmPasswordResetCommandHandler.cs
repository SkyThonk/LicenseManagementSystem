using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Contracts.Authentication;

namespace TenantService.Application.Authentication.Command.ConfirmPasswordReset;

public class ConfirmPasswordResetCommandHandler : ICommandHandler<ConfirmPasswordResetRequest, ConfirmPasswordResetResponse>
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPasswordResetCommandHandler(
        IPasswordResetTokenRepository tokenRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConfirmPasswordResetResponse>> Handle(ConfirmPasswordResetRequest request, CancellationToken ct)
    {
        // Find token
        var resetToken = await _tokenRepository.GetByTokenAsync(request.Token, ct);
        if (resetToken is null)
        {
            return Result<ConfirmPasswordResetResponse>.Failure(
                new ValidationError("Invalid or expired reset token"));
        }

        // Validate token
        if (!resetToken.IsValid())
        {
            return Result<ConfirmPasswordResetResponse>.Failure(
                new ValidationError("Invalid or expired reset token"));
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(resetToken.UserId, ct);
        if (user is null)
        {
            return Result<ConfirmPasswordResetResponse>.Failure(
                new NotFoundError("User not found"));
        }

        // Hash new password
        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

        // Update password
        user.UpdatePassword(newPasswordHash);
        _userRepository.Update(user);

        // Mark token as used
        resetToken.MarkAsUsed();
        _tokenRepository.Update(resetToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ConfirmPasswordResetResponse>.Success(new ConfirmPasswordResetResponse(
            true,
            "Password reset successfully"
        ));
    }
}
