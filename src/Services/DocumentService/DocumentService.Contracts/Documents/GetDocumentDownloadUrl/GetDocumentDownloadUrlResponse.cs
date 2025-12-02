namespace DocumentService.Contracts.Documents.GetDocumentDownloadUrl;

public sealed record GetDocumentDownloadUrlResponse(
    Guid Id,
    string DownloadUrl,
    DateTime ExpiresAt);
