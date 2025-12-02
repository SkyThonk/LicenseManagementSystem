using Common.Application.Result;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Contracts.Documents.GetDocuments;

namespace DocumentService.Application.Documents.Queries.GetDocuments;

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
        IReadOnlyList<Domain.Documents.Document> documents;
        int totalCount;

        if (request.LicenseId.HasValue)
        {
            documents = await _documentRepository.GetByLicenseIdAsync(
                request.LicenseId.Value,
                cancellationToken);
            totalCount = documents.Count;
        }
        else if (!string.IsNullOrEmpty(request.DocumentType))
        {
            documents = await _documentRepository.GetByDocumentTypeAsync(
                request.DocumentType,
                cancellationToken);
            totalCount = documents.Count;
        }
        else
        {
            documents = await _documentRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                cancellationToken);
            totalCount = await _documentRepository.GetTotalCountAsync(cancellationToken);
        }

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
