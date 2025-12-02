namespace DocumentService.Contracts.Documents.UploadDocument;

public sealed record UploadDocumentRequest(
    Guid LicenseId,
    string DocumentType,
    string FileName,
    string MimeType,
    int? SizeInKb,
    Stream FileContent);
