using TenantService.Contracts.Authentication;
using Common.Application.Result;
using Wolverine;
using Microsoft.AspNetCore.Mvc;
using Identity.Api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace TenantService.Api.Controllers;

/// <summary>
/// API Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public AuthenticationController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// User login - generates JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<LoginResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Request password reset - sends reset token to email
    /// </summary>
    [HttpPost("password-reset/request")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<RequestPasswordResetResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Confirm password reset with token
    /// </summary>
    [HttpPost("password-reset/confirm")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<ConfirmPasswordResetResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
