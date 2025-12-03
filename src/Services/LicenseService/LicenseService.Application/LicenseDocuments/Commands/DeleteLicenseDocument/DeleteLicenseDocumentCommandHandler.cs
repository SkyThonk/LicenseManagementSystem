using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseDocuments.DeleteLicenseDocument;
using LicenseService.Domain.LicenseDocuments;

namespace LicenseService.Application.LicenseDocuments.Commands.DeleteLicenseDocument;

/// <summary>
/// Handler for deleting a license document
/// </summary>
public class DeleteLicenseDocumentCommandHandler : ICommandHandler<DeleteLicenseDocumentRequest, DeleteLicenseDocumentResponse>
{
    private readonly ILicenseDocumentRepository _licenseDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLicenseDocumentCommandHandler(
        ILicenseDocumentRepository licenseDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseDocumentRepository = licenseDocumentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteLicenseDocumentResponse>> Handle(DeleteLicenseDocumentRequest request, CancellationToken ct)
    {
        var document = await _licenseDocumentRepository.GetByIdAsync(new LicenseDocumentId(request.Id), ct);
        if (document is null)
        {
            return Result<DeleteLicenseDocumentResponse>.Failure(new NotFoundError("Document not found"));
        }

        _licenseDocumentRepository.Delete(document);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new DeleteLicenseDocumentResponse(
            Id: document.Id.Value,
            Success: true,
            Message: "Document deleted successfully"
        );

        return Result<DeleteLicenseDocumentResponse>.Success(response);
    }
}
