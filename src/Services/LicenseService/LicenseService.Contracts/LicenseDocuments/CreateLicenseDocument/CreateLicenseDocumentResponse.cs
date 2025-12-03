namespace LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;

/// <summary>
/// Response after creating a new license document metadata
/// </summary>
public record CreateLicenseDocumentResponse(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt,
    DateTime CreatedAt
);
