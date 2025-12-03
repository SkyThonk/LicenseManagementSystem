using System.ComponentModel.DataAnnotations;

namespace DocumentService.Contracts.Documents.GetDocumentDownloadUrl;

public sealed record GetDocumentDownloadUrlRequest(
    [Required(ErrorMessage = "Document ID is required")]
    Guid Id
);
