using DocumentService.Contracts.Documents.DeleteDocument;

namespace DocumentService.Application.Documents.Commands.DeleteDocument;

public sealed record DeleteDocumentCommand(DeleteDocumentRequest Request);
