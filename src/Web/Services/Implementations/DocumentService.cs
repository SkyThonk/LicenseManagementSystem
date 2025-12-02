using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Documents;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly IApiService _apiService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IApiService apiService, ILogger<DocumentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public Task<DocumentListViewModel> GetDocumentsAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        // TODO: Call actual API
        var documents = new List<DocumentItemViewModel>
        {
            new DocumentItemViewModel
            {
                Id = Guid.NewGuid(),
                FileName = "Business_License.pdf",
                DocumentType = "License Agreement",
                MimeType = "application/pdf",
                SizeInKb = 245,
                UploadedAt = DateTime.UtcNow.AddDays(-10),
                UploadedByName = "John Smith",
                LicenseNumber = "LIC-2024-001"
            },
            new DocumentItemViewModel
            {
                Id = Guid.NewGuid(),
                FileName = "ID_Verification.pdf",
                DocumentType = "Identity Document",
                MimeType = "application/pdf",
                SizeInKb = 128,
                UploadedAt = DateTime.UtcNow.AddDays(-8),
                UploadedByName = "Jane Doe",
                LicenseNumber = "LIC-2024-002"
            },
            new DocumentItemViewModel
            {
                Id = Guid.NewGuid(),
                FileName = "Insurance_Certificate.jpg",
                DocumentType = "Insurance",
                MimeType = "image/jpeg",
                SizeInKb = 512,
                UploadedAt = DateTime.UtcNow.AddDays(-5),
                UploadedByName = "Bob Wilson",
                LicenseNumber = "LIC-2024-003"
            },
            new DocumentItemViewModel
            {
                Id = Guid.NewGuid(),
                FileName = "Contract_Agreement.docx",
                DocumentType = "Contract",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                SizeInKb = 89,
                UploadedAt = DateTime.UtcNow.AddDays(-2),
                UploadedByName = "Alice Brown",
                LicenseNumber = "LIC-2024-004"
            }
        };

        return Task.FromResult(new DocumentListViewModel
        {
            Documents = new PaginatedList<DocumentItemViewModel>(documents, 4, page, pageSize),
            SearchQuery = search
        });
    }

    public Task<bool> UploadDocumentAsync(DocumentUploadViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Call actual API
            _logger.LogInformation("Uploading document: {FileName} for license: {LicenseId}", 
                model.File?.FileName, model.LicenseId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Call actual API
            _logger.LogInformation("Deleting document: {Id}", id);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document");
            return Task.FromResult(false);
        }
    }

    public Task<string?> GetDownloadUrlAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Call actual API
        return Task.FromResult<string?>($"/api/documents/{id}/download");
    }
}
