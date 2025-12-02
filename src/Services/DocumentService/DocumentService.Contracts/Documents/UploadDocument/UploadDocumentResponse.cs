namespace DocumentService.Contracts.Documents.UploadDocument;

public sealed record UploadDocumentResponse(
    Guid Id,
    Guid LicenseId,
    string DocumentType,
    string FileName,
    string FileUrl,
    string MimeType,
    int? SizeInKb,
    Guid UploadedBy,
    DateTime UploadedAt);
