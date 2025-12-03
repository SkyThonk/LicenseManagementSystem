using System.ComponentModel.DataAnnotations;

namespace DocumentService.Contracts.Documents.GetDocument;

public sealed record GetDocumentRequest(
    [Required(ErrorMessage = "Document ID is required")]
    Guid Id
);
