namespace DocumentService.Application.Common.Interfaces.Services;

/// <summary>
/// Interface for file storage operations (S3, Azure Blob, local, etc.)
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload a file and return the URL
    /// </summary>
    Task<string> UploadAsync(Guid documentId, string fileName, Stream content, string mimeType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file by URL
    /// </summary>
    Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a presigned URL for downloading
    /// </summary>
    Task<string> GetDownloadUrlAsync(string fileUrl, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default);
}
