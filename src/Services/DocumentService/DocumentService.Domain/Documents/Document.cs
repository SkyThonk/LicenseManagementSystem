using Common.Domain.Abstractions;

namespace DocumentService.Domain.Documents;

/// <summary>
/// Represents a document uploaded for a license application.
/// Core entity of the DocumentService.
/// Note: No TenantId as each tenant has their own database.
/// </summary>
public sealed class Document : Entity<DocumentId>
{
    private Document(
        DocumentId id,
        Guid licenseId,
        string documentType,
        string fileName,
        string fileUrl,
        string mimeType,
        int? sizeInKb,
        Guid uploadedBy
    ) : base(id, uploadedBy)
    {
        LicenseId = licenseId;
        DocumentType = documentType;
        FileName = fileName;
        FileUrl = fileUrl;
        MimeType = mimeType;
        SizeInKb = sizeInKb;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
    }

    // For EF Core
    private Document() { }

    /// <summary>
    /// Reference to the license this document belongs to
    /// </summary>
    public Guid LicenseId { get; private set; }

    /// <summary>
    /// Type of document (e.g., ID Proof, Tax Receipt, Certificate)
    /// </summary>
    public string DocumentType { get; private set; } = null!;

    /// <summary>
    /// Original file name as uploaded
    /// </summary>
    public string FileName { get; private set; } = null!;

    /// <summary>
    /// URL/path to the stored file (S3, Azure Blob, or local)
    /// </summary>
    public string FileUrl { get; private set; } = null!;

    /// <summary>
    /// MIME type of the file (e.g., image/png, application/pdf)
    /// </summary>
    public string MimeType { get; private set; } = null!;

    /// <summary>
    /// File size in kilobytes (optional)
    /// </summary>
    public int? SizeInKb { get; private set; }

    /// <summary>
    /// User who uploaded the document
    /// </summary>
    public Guid UploadedBy { get; private set; }

    /// <summary>
    /// When the document was uploaded
    /// </summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Creates a new document record
    /// </summary>
    public static Document Create(
        Guid licenseId,
        string documentType,
        string fileName,
        string fileUrl,
        string mimeType,
        int? sizeInKb = null,
        Guid? uploadedBy = null)
    {
        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Document type is required.", nameof(documentType));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("File URL is required.", nameof(fileUrl));

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("MIME type is required.", nameof(mimeType));

        return new Document(
            DocumentId.New(),
            licenseId,
            documentType,
            fileName,
            fileUrl,
            mimeType,
            sizeInKb,
            uploadedBy ?? Guid.Empty);
    }

    /// <summary>
    /// Updates the file URL (e.g., after moving to permanent storage)
    /// </summary>
    public void UpdateFileUrl(string newFileUrl)
    {
        if (string.IsNullOrWhiteSpace(newFileUrl))
            throw new ArgumentException("File URL is required.", nameof(newFileUrl));

        FileUrl = newFileUrl;
    }

    /// <summary>
    /// Updates document metadata
    /// </summary>
    public void UpdateMetadata(string? documentType = null, string? fileName = null)
    {
        if (!string.IsNullOrWhiteSpace(documentType))
            DocumentType = documentType;

        if (!string.IsNullOrWhiteSpace(fileName))
            FileName = fileName;
    }
}
