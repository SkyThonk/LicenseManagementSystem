using Common.Application.Result;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Contracts.Documents.GetDocuments;

namespace DocumentService.Application.Documents.Queries.GetDocuments;

/// <summary>
/// Handler for getting paginated documents list.
/// Pagination and filtering happens at the SQL level for efficiency.
/// </summary>
public sealed class GetDocumentsQueryHandler
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Result<GetDocumentsResponse>> Handle(
        GetDocumentsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;

        // All filtering and pagination happens at SQL level
        var (documents, totalCount) = await _documentRepository.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            request.LicenseId,
            request.DocumentType,
            cancellationToken);

        var documentDtos = documents
            .Select(d => new DocumentDto(
                d.Id.Value,
                d.LicenseId,
                d.DocumentType,
                d.FileName,
                d.FileUrl,
                d.MimeType,
                d.SizeInKb,
                d.UploadedBy,
                d.UploadedAt))
            .ToList();

        return Result<GetDocumentsResponse>.Success(new GetDocumentsResponse(
            documentDtos,
            totalCount,
            request.Page,
            request.PageSize));
    }
}
