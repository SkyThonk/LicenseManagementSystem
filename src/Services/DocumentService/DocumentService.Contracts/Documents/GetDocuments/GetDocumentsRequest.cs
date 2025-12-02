namespace DocumentService.Contracts.Documents.GetDocuments;

public sealed record GetDocumentsRequest(
    Guid? LicenseId = null,
    string? DocumentType = null,
    int Page = 1,
    int PageSize = 10);
