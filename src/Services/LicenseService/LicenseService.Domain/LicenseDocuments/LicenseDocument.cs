using Common.Domain.Abstractions;
using LicenseService.Domain.Licenses;

namespace LicenseService.Domain.LicenseDocuments;

/// <summary>
/// Represents document metadata for a license.
/// The actual file is stored in DocumentService; this contains only metadata.
/// Each tenant has their own isolated database, so TenantId is not stored in the entity.
/// </summary>
public sealed class LicenseDocument : Entity<LicenseDocumentId>
{
    private LicenseDocument(
        LicenseDocumentId id,
        LicenseId licenseId,
        string documentType,
        string fileUrl,
        DateTime uploadedAt,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        LicenseId = licenseId;
        DocumentType = documentType;
        FileUrl = fileUrl;
        UploadedAt = uploadedAt;
    }

    // For EF Core
    private LicenseDocument() { }

    /// <summary>
    /// The license this document is associated with
    /// </summary>
    public LicenseId LicenseId { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the license
    /// </summary>
    public License? License { get; private set; }

    /// <summary>
    /// Type of document (e.g., "ID Card", "Certificate", "Photo")
    /// </summary>
    public string DocumentType { get; private set; } = null!;

    /// <summary>
    /// URL to the file in DocumentService
    /// </summary>
    public string FileUrl { get; private set; } = null!;

    /// <summary>
    /// When the document was uploaded
    /// </summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Creates a new license document
    /// </summary>
    public static LicenseDocument Create(
        LicenseId licenseId,
        string documentType,
        string fileUrl,
        Guid? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Document type is required.", nameof(documentType));

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("File URL is required.", nameof(fileUrl));

        return new LicenseDocument(
            new LicenseDocumentId(Guid.NewGuid()),
            licenseId,
            documentType,
            fileUrl,
            DateTime.UtcNow,
            createdBy
        );
    }

    /// <summary>
    /// Updates the file URL (e.g., when document is re-uploaded)
    /// </summary>
    public void UpdateFileUrl(string newFileUrl)
    {
        if (string.IsNullOrWhiteSpace(newFileUrl))
            throw new ArgumentException("File URL is required.", nameof(newFileUrl));

        FileUrl = newFileUrl;
        UploadedAt = DateTime.UtcNow;
    }
}
