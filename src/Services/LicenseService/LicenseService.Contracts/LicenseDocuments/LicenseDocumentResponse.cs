namespace LicenseService.Contracts.LicenseDocuments;

public record LicenseDocumentResponse(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt,
    DateTime CreatedAt
);
