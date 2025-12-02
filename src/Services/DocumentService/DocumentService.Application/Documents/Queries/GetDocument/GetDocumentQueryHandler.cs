using Common.Application.Result;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Contracts.Documents.GetDocument;
using DocumentService.Domain.Documents;

namespace DocumentService.Application.Documents.Queries.GetDocument;

public sealed class GetDocumentQueryHandler
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Result<GetDocumentResponse>> Handle(
        GetDocumentQuery query,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            new DocumentId(query.Request.Id),
            cancellationToken);

        if (document is null)
        {
            return Result<GetDocumentResponse>.Failure(
                new NotFoundError($"Document with ID {query.Request.Id} was not found"));
        }

        return Result<GetDocumentResponse>.Success(new GetDocumentResponse(
            document.Id.Value,
            document.LicenseId,
            document.DocumentType,
            document.FileName,
            document.FileUrl,
            document.MimeType,
            document.SizeInKb,
            document.UploadedBy,
            document.UploadedAt));
    }
}
