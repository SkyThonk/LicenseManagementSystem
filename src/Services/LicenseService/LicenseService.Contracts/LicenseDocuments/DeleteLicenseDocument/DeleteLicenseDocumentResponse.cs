namespace LicenseService.Contracts.LicenseDocuments.DeleteLicenseDocument;

/// <summary>
/// Response after deleting a license document
/// </summary>
public record DeleteLicenseDocumentResponse(
    Guid Id,
    bool Success,
    string Message
);
