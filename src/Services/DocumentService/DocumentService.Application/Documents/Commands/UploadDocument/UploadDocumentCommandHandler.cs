using Common.Application.Result;
using DocumentService.Application.Common.Interfaces;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Application.Common.Interfaces.Services;
using DocumentService.Contracts.Documents.UploadDocument;
using DocumentService.Domain.Documents;

namespace DocumentService.Application.Documents.Commands.UploadDocument;

public sealed class UploadDocumentCommandHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UploadDocumentResponse>> Handle(
        UploadDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var documentId = Guid.NewGuid();
        
        // Upload file to storage
        var fileUrl = await _fileStorageService.UploadAsync(
            documentId,
            command.Request.FileName,
            command.Request.FileContent,
            command.Request.MimeType,
            cancellationToken);

        if (string.IsNullOrEmpty(fileUrl))
        {
            return Result<UploadDocumentResponse>.Failure(
                new UnexpectedError("Failed to upload file to storage"));
        }

        var document = Document.Create(
            command.Request.LicenseId,
            command.Request.DocumentType,
            command.Request.FileName,
            fileUrl,
            command.Request.MimeType,
            command.Request.SizeInKb,
            command.UserId);

        _documentRepository.Add(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UploadDocumentResponse>.Success(new UploadDocumentResponse(
            document.Id.Value,
            document.LicenseId,
            document.DocumentType,
            document.FileName,
            document.FileUrl,
            document.MimeType,
            document.SizeInKb,
            document.UploadedBy,
            document.UploadedAt));
    }
}
