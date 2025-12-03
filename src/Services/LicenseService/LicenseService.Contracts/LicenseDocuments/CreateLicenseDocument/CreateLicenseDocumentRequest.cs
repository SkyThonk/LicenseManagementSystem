using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;

/// <summary>
/// Request to create a new license document metadata.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateLicenseDocumentRequest(
    [Required]
    Guid LicenseId,

    [Required]
    [MaxLength(100)]
    string DocumentType,

    [Required]
    [MaxLength(500)]
    string FileUrl
);
