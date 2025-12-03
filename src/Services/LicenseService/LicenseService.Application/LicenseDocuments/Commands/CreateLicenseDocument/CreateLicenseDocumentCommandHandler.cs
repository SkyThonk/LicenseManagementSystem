using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseDocuments.CreateLicenseDocument;
using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.LicenseDocuments.Commands.CreateLicenseDocument;

/// <summary>
/// Handler for creating a new license document metadata
/// </summary>
public class CreateLicenseDocumentCommandHandler : ICommandHandler<CreateLicenseDocumentRequest, CreateLicenseDocumentResponse>
{
    private readonly ILicenseDocumentRepository _licenseDocumentRepository;
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLicenseDocumentCommandHandler(
        ILicenseDocumentRepository licenseDocumentRepository,
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseDocumentRepository = licenseDocumentRepository;
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateLicenseDocumentResponse>> Handle(CreateLicenseDocumentRequest request, CancellationToken ct)
    {
        // Validate license exists
        var license = await _licenseRepository.GetByIdAsync(new LicenseId(request.LicenseId), ct);
        if (license is null)
        {
            return Result<CreateLicenseDocumentResponse>.Failure(new NotFoundError("License not found"));
        }

        // Create the document metadata
        var document = LicenseDocument.Create(
            request.TenantId,
            new LicenseId(request.LicenseId),
            request.DocumentType,
            request.FileUrl
        );

        _licenseDocumentRepository.Add(document);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new CreateLicenseDocumentResponse(
            Id: document.Id.Value,
            TenantId: document.TenantId,
            LicenseId: document.LicenseId.Value,
            DocumentType: document.DocumentType,
            FileUrl: document.FileUrl,
            UploadedAt: document.UploadedAt,
            CreatedAt: document.CreatedAt
        );

        return Result<CreateLicenseDocumentResponse>.Success(response);
    }
}
