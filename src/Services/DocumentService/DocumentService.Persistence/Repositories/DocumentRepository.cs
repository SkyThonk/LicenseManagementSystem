using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Domain.Documents;
using DocumentService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Persistence.Repositories;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly DataContext _context;

    public DocumentRepository(DataContext context)
    {
        _context = context;
    }

    public void Add(Document document)
    {
        _context.Documents.Add(document);
    }

    public void Update(Document document)
    {
        _context.Documents.Update(document);
    }

    public void Delete(Document document)
    {
        document.IsDeleted = true;
        _context.Documents.Update(document);
    }

    public async Task<Document?> GetByIdAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .Where(d => d.LicenseId == licenseId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .Where(d => d.DocumentType == documentType)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByUploadedByAsync(Guid uploadedBy, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .Where(d => d.UploadedBy == uploadedBy)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .OrderByDescending(d => d.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Documents.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents.AnyAsync(d => d.Id == id, cancellationToken);
    }
}
