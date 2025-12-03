using System.ComponentModel.DataAnnotations;

namespace DocumentService.Contracts.Documents.GetDocuments;

public sealed record GetDocumentsRequest(
    Guid? LicenseId = null,

    [MaxLength(100, ErrorMessage = "Document type cannot exceed 100 characters")]
    string? DocumentType = null,

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10
);
