using System.ComponentModel.DataAnnotations;

namespace DocumentService.Contracts.Documents.UploadDocument;

public sealed record UploadDocumentRequest(
    [Required(ErrorMessage = "License ID is required")]
    Guid LicenseId,

    [Required(ErrorMessage = "Document type is required")]
    [MaxLength(100, ErrorMessage = "Document type cannot exceed 100 characters")]
    string DocumentType,

    [Required(ErrorMessage = "File name is required")]
    [MaxLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
    string FileName,

    [Required(ErrorMessage = "MIME type is required")]
    [MaxLength(100, ErrorMessage = "MIME type cannot exceed 100 characters")]
    string MimeType,

    [Range(0, int.MaxValue, ErrorMessage = "Size must be non-negative")]
    int? SizeInKb,

    [Required(ErrorMessage = "File content is required")]
    Stream FileContent
);
