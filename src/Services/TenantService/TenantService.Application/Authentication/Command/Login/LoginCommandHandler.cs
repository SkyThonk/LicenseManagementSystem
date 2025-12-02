using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Contracts.Authentication;

namespace TenantService.Application.Authentication.Command.Login;

public class LoginCommandHandler : ICommandHandler<LoginRequest, LoginResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, ct);

        if (user is null)
        {
            return Result<LoginResponse>.Failure(new ValidationError("Invalid email or password"));
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result<LoginResponse>.Failure(new ValidationError("User account is blocked"));
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(new ValidationError("Invalid email or password"));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse(
            user.Id.Value,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            token
        ));
    }
}
