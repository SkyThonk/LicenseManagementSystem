using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;

/// <summary>
/// Request to create a new license document metadata.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateLicenseDocumentRequest(
    [Required(ErrorMessage = "License ID is required")]
    Guid LicenseId,

    [Required(ErrorMessage = "Document type is required")]
    [MaxLength(100, ErrorMessage = "Document type cannot exceed 100 characters")]
    string DocumentType,

    [Required(ErrorMessage = "File URL is required")]
    [MaxLength(500, ErrorMessage = "File URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Invalid URL format")]
    string FileUrl
);
