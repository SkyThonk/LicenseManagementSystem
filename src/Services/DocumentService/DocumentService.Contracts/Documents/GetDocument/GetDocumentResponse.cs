namespace DocumentService.Contracts.Documents.GetDocument;

public sealed record GetDocumentResponse(
    Guid Id,
    Guid LicenseId,
    string DocumentType,
    string FileName,
    string FileUrl,
    string MimeType,
    int? SizeInKb,
    Guid UploadedBy,
    DateTime UploadedAt);
