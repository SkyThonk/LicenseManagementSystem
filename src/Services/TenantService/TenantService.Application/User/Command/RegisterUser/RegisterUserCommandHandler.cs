using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Application.Common.Interfaces.Services;
using TenantService.Contracts.User.RegisterUser;
using TenantService.Domain.User;
using TenantService.Domain.Tenant;

namespace TenantService.Application.User.Command.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterUserResponse>> Handle(RegisterUserRequest request, CancellationToken ct)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (existingUser is not null)
        {
            return Result<RegisterUserResponse>.Failure(
                new ValidationError("A user with this email already exists"));
        }

        // Parse and validate role
        if (!Enum.TryParse<Role>(request.Role, true, out var role))
        {
            return Result<RegisterUserResponse>.Failure(
                new ValidationError($"Invalid role: {request.Role}. Valid roles are: TenantAdmin, Admin, User"));
        }

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Create user
        var user = Domain.User.User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            role,
            new TenantId(request.TenantId)
        );

        // Save to database
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(ct);

        // Send welcome email (fire and forget - don't block on this)
        _ = _emailService.SendWelcomeEmailAsync(
            user.Email, 
            user.FirstName ?? "User", 
            ct);

        return Result<RegisterUserResponse>.Success(new RegisterUserResponse(
            user.Id.Value,
            user.Email,
            user.Role.ToString(),
            user.CreatedAt
        ));
    }
}
