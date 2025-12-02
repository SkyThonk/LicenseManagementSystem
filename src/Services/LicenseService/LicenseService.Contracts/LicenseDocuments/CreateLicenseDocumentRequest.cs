namespace LicenseService.Contracts.LicenseDocuments;

public record CreateLicenseDocumentRequest(
    Guid TenantId,
    Guid LicenseId,
    string DocumentType,
    string FileUrl
);
