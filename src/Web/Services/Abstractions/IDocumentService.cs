using LicenseManagement.Web.ViewModels.Documents;

namespace LicenseManagement.Web.Services.Abstractions;

public interface IDocumentService
{
    Task<DocumentListViewModel> GetDocumentsAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default);
    Task<bool> UploadDocumentAsync(DocumentUploadViewModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string?> GetDownloadUrlAsync(Guid id, CancellationToken cancellationToken = default);
}
