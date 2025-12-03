using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.DeleteLicenseDocument;

/// <summary>
/// Request to delete a license document
/// </summary>
public record DeleteLicenseDocumentRequest(
    [Required]
    Guid Id
);
