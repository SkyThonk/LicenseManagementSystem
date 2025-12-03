using Common.Application.Result;
using LicenseService.Api.Extensions;
using LicenseService.Contracts.Renewals.CreateRenewal;
using LicenseService.Contracts.Renewals.GetRenewal;
using LicenseService.Contracts.Renewals.GetRenewalList;
using LicenseService.Contracts.Renewals.ProcessRenewal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace LicenseService.Api.Controllers;

/// <summary>
/// API Controller for managing license renewals
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RenewalsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public RenewalsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Create a new renewal request
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRenewalRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<CreateRenewalResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get renewal by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetRenewalRequest(id);
        var result = await _messageBus.InvokeAsync<Result<RenewalDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of renewals
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetRenewalListRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetRenewalListResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Process a renewal (approve or reject)
    /// </summary>
    [HttpPatch("{id:guid}/process")]
    public async Task<IActionResult> Process(
        [FromRoute] Guid id, 
        [FromBody] ProcessRenewalRequest request, 
        CancellationToken ct)
    {
        var updated = request with { Id = id };
        var result = await _messageBus.InvokeAsync<Result<ProcessRenewalResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Approve a renewal
    /// </summary>
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new ProcessRenewalRequest(id, true);
        var result = await _messageBus.InvokeAsync<Result<ProcessRenewalResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Reject a renewal
    /// </summary>
    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new ProcessRenewalRequest(id, false);
        var result = await _messageBus.InvokeAsync<Result<ProcessRenewalResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
