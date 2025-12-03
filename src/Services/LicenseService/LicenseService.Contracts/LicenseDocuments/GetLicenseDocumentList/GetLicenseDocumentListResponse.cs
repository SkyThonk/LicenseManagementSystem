namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocumentList;

/// <summary>
/// DTO for license document list item
/// </summary>
public record LicenseDocumentListItemDto(
    Guid Id,
    Guid TenantId,
    Guid LicenseId,
    string DocumentType,
    string FileUrl,
    DateTime UploadedAt
);

/// <summary>
/// Response for license document list
/// </summary>
public record GetLicenseDocumentListResponse(
    IEnumerable<LicenseDocumentListItemDto> Items
);
