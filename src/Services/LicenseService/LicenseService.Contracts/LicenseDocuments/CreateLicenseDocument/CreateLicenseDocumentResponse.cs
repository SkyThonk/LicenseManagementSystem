namespace LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;

/// <summary>
/// Response after creating a new license document metadata.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record CreateLicenseDocumentResponse(
    Guid Id,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt,
    DateTime CreatedAt
);
