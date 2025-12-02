using DocumentService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentService.Infrastructure.Services;

/// <summary>
/// Local file system storage implementation for documents.
/// In production, this would be replaced with cloud storage (Azure Blob, S3, etc.)
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _basePath;

    public LocalFileStorageService(
        IConfiguration configuration,
        ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _basePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Path.GetTempPath(), "DocumentService");
        
        // Ensure base directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadAsync(
        Guid documentId,
        string fileName,
        Stream content,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var extension = Path.GetExtension(fileName);
            var storedFileName = $"{documentId}{extension}";
            var filePath = Path.Combine(_basePath, storedFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await content.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation(
                "Uploaded file {FileName} as {StoredFileName}",
                fileName,
                storedFileName);

            // Return a relative URL that can be used to retrieve the file
            return $"/files/{storedFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", fileName);
            return string.Empty;
        }
    }

    public async Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(_basePath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted file {FileName}", fileName);
                    return true;
                }

                _logger.LogWarning("File not found for deletion: {FileUrl}", fileUrl);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {FileUrl}", fileUrl);
                return false;
            }
        }, cancellationToken);
    }

    public Task<string> GetDownloadUrlAsync(
        string fileUrl,
        DateTime expiresAt,
        CancellationToken cancellationToken = default)
    {
        // For local storage, return the same URL (no pre-signed URL concept)
        // In production with cloud storage, this would generate a pre-signed URL
        return Task.FromResult(fileUrl);
    }

    public Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_basePath, fileName);
            return Task.FromResult(File.Exists(filePath));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
