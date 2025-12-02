namespace DocumentService.Contracts.Documents.GetDocuments;

public sealed record GetDocumentsResponse(
    IReadOnlyList<DocumentDto> Documents,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record DocumentDto(
    Guid Id,
    Guid LicenseId,
    string DocumentType,
    string FileName,
    string FileUrl,
    string MimeType,
    int? SizeInKb,
    Guid UploadedBy,
    DateTime UploadedAt);
