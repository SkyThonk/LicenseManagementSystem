using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Documents;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly IDocumentApiService _apiService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IDocumentApiService apiService, ILogger<DocumentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<DocumentListViewModel> GetDocumentsAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?page={page}&pageSize={pageSize}";

            var response = await _apiService.GetAsync<GetDocumentsApiResponse>(
                $"{ApiEndpoints.Document.GetAll}{queryParams}",
                cancellationToken);

            if (response != null)
            {
                var items = response.Documents.Select(MapToListItemViewModel).ToList();
                return new DocumentListViewModel
                {
                    Documents = new PaginatedList<DocumentItemViewModel>(
                        items,
                        response.TotalCount,
                        response.Page,
                        response.PageSize),
                    SearchQuery = search
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching documents list");
        }

        return new DocumentListViewModel
        {
            Documents = new PaginatedList<DocumentItemViewModel>([], 0, page, pageSize),
            SearchQuery = search
        };
    }

    public async Task<bool> UploadDocumentAsync(DocumentUploadViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            if (model.File == null)
            {
                _logger.LogWarning("No file provided for upload");
                return false;
            }

            var response = await _apiService.UploadFileAsync(
                ApiEndpoints.Document.Upload,
                model.File,
                model.LicenseId,
                model.DocumentType ?? "Document",
                cancellationToken);

            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return false;
        }
    }

    public async Task<bool> DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Document.Delete, id);
            var response = await _apiService.DeleteAsync<DeleteDocumentApiResponse>(endpoint, cancellationToken);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return false;
        }
    }

    public Task<string?> GetDownloadUrlAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = string.Format(ApiEndpoints.Document.Download, id);
        return Task.FromResult<string?>(endpoint);
    }

    private static DocumentItemViewModel MapToListItemViewModel(DocumentApiDto dto)
    {
        return new DocumentItemViewModel
        {
            Id = dto.Id,
            FileName = dto.FileName,
            DocumentType = dto.DocumentType,
            MimeType = dto.MimeType,
            SizeInKb = dto.SizeInKb,
            UploadedAt = dto.UploadedAt,
            UploadedByName = dto.UploadedBy.ToString()[..8],
            LicenseNumber = dto.LicenseId.ToString()[..8].ToUpper()
        };
    }
}

#region API DTOs

internal class GetDocumentsApiResponse
{
    public List<DocumentApiDto> Documents { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal class DocumentApiDto
{
    public Guid Id { get; set; }
    public Guid LicenseId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public int? SizeInKb { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

internal class UploadDocumentApiResponse
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public DateTime UploadedAt { get; set; }
}

internal class DeleteDocumentApiResponse
{
    public bool Success { get; set; }
}

#endregion
