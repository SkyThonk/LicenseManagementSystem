using TenantService.Contracts.Tenant.RegisterTenant;
using TenantService.Contracts.Tenant.GetTenantProfile;
using TenantService.Contracts.Tenant.UpdateTenant;
using TenantService.Contracts.Tenant.BlockTenant;
using TenantService.Contracts.Tenant.GetTenantList;
using Common.Application.Result;
using Wolverine;
using Microsoft.AspNetCore.Mvc;
using Identity.Api.Extensions;

namespace TenantService.Api.Controllers;

/// <summary>
/// API Controller for managing government agencies (tenants)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TenantController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Register a new government agency
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterTenantRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<RegisterTenantResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get agency profile by ID
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> Profile([FromQuery] GetTenantProfileRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<TenantProfileDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Update agency profile
    /// </summary>
    [HttpPut("profile/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTenantRequest request, CancellationToken ct)
    {
        var updated = request with { Id = id };
        var result = await _messageBus.InvokeAsync<Result<UpdateTenantResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Activate a government agency
    /// </summary>
    [HttpPatch("{tenantId:guid}/activate")]
    public async Task<IActionResult> Activate([FromRoute] Guid tenantId, CancellationToken ct)
    {
        var request = new BlockTenantRequest(tenantId, true);  
        var result = await _messageBus.InvokeAsync<Result<BlockTenantResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Deactivate a government agency
    /// </summary>
    [HttpPatch("{tenantId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid tenantId, CancellationToken ct)
    {
        var request = new BlockTenantRequest(tenantId, false);  
        var result = await _messageBus.InvokeAsync<Result<BlockTenantResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of government agencies
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] GetTenantListRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetTenantListResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}

