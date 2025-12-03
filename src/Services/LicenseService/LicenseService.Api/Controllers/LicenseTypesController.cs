using Common.Application.Result;
using LicenseService.Api.Extensions;
using LicenseService.Contracts.LicenseTypes.CreateLicenseType;
using LicenseService.Contracts.LicenseTypes.DeleteLicenseType;
using LicenseService.Contracts.LicenseTypes.GetLicenseType;
using LicenseService.Contracts.LicenseTypes.GetLicenseTypeList;
using LicenseService.Contracts.LicenseTypes.UpdateLicenseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace LicenseService.Api.Controllers;

/// <summary>
/// API Controller for managing license types
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LicenseTypesController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public LicenseTypesController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Create a new license type
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLicenseTypeRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<CreateLicenseTypeResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get license type by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetLicenseTypeRequest(id);
        var result = await _messageBus.InvokeAsync<Result<LicenseTypeDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of license types
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetLicenseTypeListRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetLicenseTypeListResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Update a license type
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id, 
        [FromBody] UpdateLicenseTypeRequest request, 
        CancellationToken ct)
    {
        var updated = request with { Id = id };
        var result = await _messageBus.InvokeAsync<Result<UpdateLicenseTypeResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Delete a license type
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteLicenseTypeRequest(id);
        var result = await _messageBus.InvokeAsync<Result<DeleteLicenseTypeResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
