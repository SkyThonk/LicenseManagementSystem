using DocumentService.Domain.Documents;

namespace DocumentService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Document entity
/// </summary>
public interface IDocumentRepository
{
    void Add(Document document);
    void Update(Document document);
    void Delete(Document document);
    Task<Document?> GetByIdAsync(DocumentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByUploadedByAsync(Guid uploadedBy, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated documents with optional filters - pagination happens at SQL level
    /// </summary>
    Task<(IReadOnlyList<Document> Items, int TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        Guid? licenseId = null,
        string? documentType = null,
        CancellationToken cancellationToken = default);
}
