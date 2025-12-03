using TenantService.Contracts.User.RegisterUser;
using TenantService.Contracts.User.GetUserProfile;
using TenantService.Contracts.User.UpdateUser;
using TenantService.Contracts.User.BlockUser;
using TenantService.Contracts.User.UnblockUser;
using TenantService.Contracts.User.DeleteUser;
using TenantService.Contracts.User.GetUserList;
using Common.Application.Result;
using Wolverine;
using Microsoft.AspNetCore.Mvc;
using TenantService.Api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace TenantService.Api.Controllers;

/// <summary>
/// API Controller for managing users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class UserController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public UserController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Register a new user (Admin/TenantAdmin only)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<RegisterUserResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Profile([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetUserProfileRequest(id);
        var result = await _messageBus.InvokeAsync<Result<UserProfileDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of users
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] GetUserListRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetUserListResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var updated = request with { Id = id };
        var result = await _messageBus.InvokeAsync<Result<UpdateUserResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Block user (Admin/TenantAdmin only)
    /// </summary>
    [HttpPatch("{id:guid}/block")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Block([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new BlockUserRequest(id);
        var result = await _messageBus.InvokeAsync<Result<BlockUserResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Unblock user (Admin/TenantAdmin only)
    /// </summary>
    [HttpPatch("{id:guid}/unblock")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Unblock([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new UnblockUserRequest(id);
        var result = await _messageBus.InvokeAsync<Result<UnblockUserResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Delete user (Admin/TenantAdmin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteUserRequest(id);
        var result = await _messageBus.InvokeAsync<Result<DeleteUserResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
