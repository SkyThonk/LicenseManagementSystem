using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
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

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(new ValidationError("Invalid email or password"));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse(
            user.Id.Value,
            user.Email,
            user.Role.ToString(),
            token
        ));
    }
}
