using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocument;

/// <summary>
/// Request to get a license document by ID
/// </summary>
public record GetLicenseDocumentRequest(
    [Required]
    Guid Id
);
