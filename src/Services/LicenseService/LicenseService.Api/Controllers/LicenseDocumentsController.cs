using Common.Application.Result;
using LicenseService.Api.Extensions;
using LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;
using LicenseService.Contracts.LicenseDocuments.DeleteLicenseDocument;
using LicenseService.Contracts.LicenseDocuments.GetLicenseDocument;
using LicenseService.Contracts.LicenseDocuments.GetLicenseDocumentList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace LicenseService.Api.Controllers;

/// <summary>
/// API Controller for managing license documents
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LicenseDocumentsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public LicenseDocumentsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Create a new license document metadata
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLicenseDocumentRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<CreateLicenseDocumentResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get license document by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetLicenseDocumentRequest(id);
        var result = await _messageBus.InvokeAsync<Result<LicenseDocumentDto>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get list of documents for a license
    /// </summary>
    [HttpGet("license/{licenseId:guid}")]
    public async Task<IActionResult> ListByLicense([FromRoute] Guid licenseId, CancellationToken ct)
    {
        var request = new GetLicenseDocumentListRequest(licenseId);
        var result = await _messageBus.InvokeAsync<Result<GetLicenseDocumentListResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Delete a license document
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteLicenseDocumentRequest(id);
        var result = await _messageBus.InvokeAsync<Result<DeleteLicenseDocumentResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
