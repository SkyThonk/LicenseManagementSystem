using Common.Application.Result;
using DocumentService.Application.Common.Interfaces;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Application.Common.Interfaces.Services;
using DocumentService.Contracts.Documents.DeleteDocument;

namespace DocumentService.Application.Documents.Commands.DeleteDocument;

public sealed class DeleteDocumentCommandHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteDocumentResponse>> Handle(
        DeleteDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            new Domain.Documents.DocumentId(command.Request.Id),
            cancellationToken);

        if (document is null)
        {
            return Result<DeleteDocumentResponse>.Failure(
                new NotFoundError($"Document with ID {command.Request.Id} was not found"));
        }

        // Delete file from storage
        await _fileStorageService.DeleteAsync(document.FileUrl, cancellationToken);

        _documentRepository.Delete(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeleteDocumentResponse>.Success(new DeleteDocumentResponse(true));
    }
}
