using Common.Application.Result;
using DocumentService.Api.Extensions;
using DocumentService.Application.Documents.Commands.DeleteDocument;
using DocumentService.Application.Documents.Commands.UploadDocument;
using DocumentService.Application.Documents.Queries.GetDocument;
using DocumentService.Application.Documents.Queries.GetDocumentDownloadUrl;
using DocumentService.Application.Documents.Queries.GetDocuments;
using DocumentService.Contracts.Documents.DeleteDocument;
using DocumentService.Contracts.Documents.GetDocument;
using DocumentService.Contracts.Documents.GetDocumentDownloadUrl;
using DocumentService.Contracts.Documents.GetDocuments;
using DocumentService.Contracts.Documents.UploadDocument;
using DocumentService.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace DocumentService.Api.Controllers;

/// <summary>
/// API Controller for managing documents
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly IUserContext _userContext;

    public DocumentsController(IMessageBus messageBus, IUserContext userContext)
    {
        _messageBus = messageBus;
        _userContext = userContext;
    }

    /// <summary>
    /// Upload a new document
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(
        [FromForm] Guid licenseId,
        [FromForm] string documentType,
        IFormFile file,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        using var stream = file.OpenReadStream();
        var request = new UploadDocumentRequest(
            licenseId,
            documentType,
            file.FileName,
            file.ContentType,
            (int)(file.Length / 1024), // Convert to KB
            stream);

        var command = new UploadDocumentCommand(request, _userContext.UserId ?? Guid.Empty);
        var result = await _messageBus.InvokeAsync<Result<UploadDocumentResponse>>(command, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get document by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocument([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetDocumentRequest(id);
        var query = new GetDocumentQuery(request);
        var result = await _messageBus.InvokeAsync<Result<GetDocumentResponse>>(query, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get download URL for a document (with expiry)
    /// </summary>
    [HttpGet("{id:guid}/download-url")]
    public async Task<IActionResult> GetDocumentDownloadUrl([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetDocumentDownloadUrlRequest(id);
        var query = new GetDocumentDownloadUrlQuery(request);
        var result = await _messageBus.InvokeAsync<Result<GetDocumentDownloadUrlResponse>>(query, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of documents
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] GetDocumentsRequest request, CancellationToken ct)
    {
        var query = new GetDocumentsQuery(request);
        var result = await _messageBus.InvokeAsync<Result<GetDocumentsResponse>>(query, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get documents by license ID
    /// </summary>
    [HttpGet("license/{licenseId:guid}")]
    public async Task<IActionResult> GetDocumentsByLicense([FromRoute] Guid licenseId, CancellationToken ct)
    {
        var request = new GetDocumentsRequest(licenseId);
        var query = new GetDocumentsQuery(request);
        var result = await _messageBus.InvokeAsync<Result<GetDocumentsResponse>>(query, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Delete a document
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteDocumentRequest(id);
        var command = new DeleteDocumentCommand(request);
        var result = await _messageBus.InvokeAsync<Result<DeleteDocumentResponse>>(command, ct);
        return result.ToActionResult(this);
    }
}
