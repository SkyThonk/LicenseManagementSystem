namespace LicenseService.Contracts.LicenseDocuments.GetLicenseDocumentList;

/// <summary>
/// DTO for license document list item.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseDocumentListItemDto(
    Guid Id,
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
