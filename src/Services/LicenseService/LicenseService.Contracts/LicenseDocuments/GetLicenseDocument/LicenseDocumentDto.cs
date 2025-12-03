namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocument;

/// <summary>
/// License document details DTO.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseDocumentDto(
    Guid Id,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt,
    DateTime CreatedAt
);
