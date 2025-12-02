using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Documents;

/// <summary>
/// Document list page view model
/// </summary>
public class DocumentListViewModel : BaseViewModel
{
    public PaginatedList<DocumentItemViewModel> Documents { get; set; } = new();
    public string? SearchQuery { get; set; }
    public string? TypeFilter { get; set; }
    public Guid? LicenseId { get; set; }
}

/// <summary>
/// Individual document item for list display
/// </summary>
public class DocumentItemViewModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public int? SizeInKb { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
}
