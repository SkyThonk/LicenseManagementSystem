using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocumentList;

/// <summary>
/// Request for list of license documents by license ID
/// </summary>
public record GetLicenseDocumentListRequest(
    [Required]
    Guid LicenseId
);
