namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocument;

/// <summary>
/// License document details DTO
/// </summary>
public record LicenseDocumentDto(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt,
    DateTime CreatedAt
);
