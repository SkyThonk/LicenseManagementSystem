using Common.Application.Result;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Application.Common.Interfaces.Services;
using DocumentService.Contracts.Documents.GetDocumentDownloadUrl;
using DocumentService.Domain.Documents;

namespace DocumentService.Application.Documents.Queries.GetDocumentDownloadUrl;

public sealed class GetDocumentDownloadUrlQueryHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;

    public GetDocumentDownloadUrlQueryHandler(
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<GetDocumentDownloadUrlResponse>> Handle(
        GetDocumentDownloadUrlQuery query,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            new DocumentId(query.Request.Id),
            cancellationToken);

        if (document is null)
        {
            return Result<GetDocumentDownloadUrlResponse>.Failure(
                new NotFoundError($"Document with ID {query.Request.Id} was not found"));
        }

        var expiresAt = DateTime.UtcNow.AddHours(1);
        var downloadUrl = await _fileStorageService.GetDownloadUrlAsync(
            document.FileUrl,
            expiresAt,
            cancellationToken);

        if (string.IsNullOrEmpty(downloadUrl))
        {
            return Result<GetDocumentDownloadUrlResponse>.Failure(
                new UnexpectedError("Failed to generate download URL"));
        }

        return Result<GetDocumentDownloadUrlResponse>.Success(new GetDocumentDownloadUrlResponse(
            document.Id.Value,
            downloadUrl,
            expiresAt));
    }
}
