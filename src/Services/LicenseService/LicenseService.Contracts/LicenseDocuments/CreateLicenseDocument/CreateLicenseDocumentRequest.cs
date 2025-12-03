using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;

/// <summary>
/// Request to create a new license document metadata
/// </summary>
public record CreateLicenseDocumentRequest(
    [Required]
    Guid TenantId,

    [Required]
    Guid LicenseId,

    [Required]
    [MaxLength(100)]
    string DocumentType,

    [Required]
    [MaxLength(500)]
    string FileUrl
);
