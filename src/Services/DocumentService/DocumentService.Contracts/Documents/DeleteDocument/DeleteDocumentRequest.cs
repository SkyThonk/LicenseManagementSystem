using System.ComponentModel.DataAnnotations;

namespace DocumentService.Contracts.Documents.DeleteDocument;

public sealed record DeleteDocumentRequest(
    [Required(ErrorMessage = "Document ID is required")]
    Guid Id
);
