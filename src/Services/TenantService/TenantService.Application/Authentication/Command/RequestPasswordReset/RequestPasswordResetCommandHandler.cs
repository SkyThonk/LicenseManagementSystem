using TenantService.Application.Common.Interfaces.Services;
using TenantService.Contracts.Authentication;
using TenantService.Domain.PasswordResetToken;

namespace TenantService.Application.Authentication.Command.RequestPasswordReset;

public class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetRequest, RequestPasswordResetResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RequestPasswordResetCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RequestPasswordResetResponse>> Handle(RequestPasswordResetRequest request, CancellationToken ct)
    {
        // Always return success to prevent email enumeration attacks
        const string successMessage = "If your email is registered, you will receive a password reset link shortly.";

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
        {
            // Don't reveal that email doesn't exist
            return Result<RequestPasswordResetResponse>.Success(new RequestPasswordResetResponse(
                true,
                successMessage
            ));
        }

        // Check if user is active
        if (!user.IsActive)
        {
            // Don't reveal that user is blocked
            return Result<RequestPasswordResetResponse>.Success(new RequestPasswordResetResponse(
                true,
                successMessage
            ));
        }

        // Create password reset token
        var resetToken = PasswordResetToken.Create(user.Id, expirationHours: 1);
        _tokenRepository.Add(resetToken);
        await _unitOfWork.SaveChangesAsync(ct);

        // Send reset email (fire and forget)
        _ = _emailService.SendPasswordResetEmailAsync(
            user.Email,
            resetToken.Token,
            user.FirstName ?? "User",
            ct);

        return Result<RequestPasswordResetResponse>.Success(new RequestPasswordResetResponse(
            true,
            successMessage
        ));
    }
}
