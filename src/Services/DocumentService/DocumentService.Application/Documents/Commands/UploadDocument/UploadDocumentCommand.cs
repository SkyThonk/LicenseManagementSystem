using DocumentService.Contracts.Documents.UploadDocument;

namespace DocumentService.Application.Documents.Commands.UploadDocument;

public sealed record UploadDocumentCommand(
    UploadDocumentRequest Request,
    Guid UserId);
