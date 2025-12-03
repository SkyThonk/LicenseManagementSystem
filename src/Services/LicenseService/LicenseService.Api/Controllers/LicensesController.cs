using Common.Application.Result;
using LicenseService.Api.Extensions;
using LicenseService.Contracts.Licenses.CreateLicense;
using LicenseService.Contracts.Licenses.GetLicense;
using LicenseService.Contracts.Licenses.GetLicenseList;
using LicenseService.Contracts.Licenses.UpdateLicenseStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace LicenseService.Api.Controllers;

/// <summary>
/// API Controller for managing licenses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LicensesController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public LicensesController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Create a new license application
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLicenseRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<CreateLicenseResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get license by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetLicenseRequest(id);
        var result = await _messageBus.InvokeAsync<Result<LicenseDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of licenses
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetLicenseListRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetLicenseListResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Update license status (approve, reject, etc.)
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id, 
        [FromBody] UpdateLicenseStatusRequest request, 
        CancellationToken ct)
    {
        var updated = request with { Id = id };
        var result = await _messageBus.InvokeAsync<Result<UpdateLicenseStatusResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Approve a license
    /// </summary>
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        [FromRoute] Guid id, 
        [FromQuery] DateTime? expiryDate, 
        CancellationToken ct)
    {
        var request = new UpdateLicenseStatusRequest(id, "Approved", expiryDate);
        var result = await _messageBus.InvokeAsync<Result<UpdateLicenseStatusResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Reject a license
    /// </summary>
    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new UpdateLicenseStatusRequest(id, "Rejected", null);
        var result = await _messageBus.InvokeAsync<Result<UpdateLicenseStatusResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
