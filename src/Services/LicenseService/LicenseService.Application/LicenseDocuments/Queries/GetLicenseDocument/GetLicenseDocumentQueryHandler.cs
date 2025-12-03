using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseDocuments.GetLicenseDocument;
using LicenseService.Domain.LicenseDocuments;

namespace LicenseService.Application.LicenseDocuments.Queries.GetLicenseDocument;

/// <summary>
/// Handler for getting a license document by ID.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// </summary>
public class GetLicenseDocumentQueryHandler : IQueryHandler<GetLicenseDocumentRequest, LicenseDocumentDto>
{
    private readonly ILicenseDocumentRepository _licenseDocumentRepository;

    public GetLicenseDocumentQueryHandler(ILicenseDocumentRepository licenseDocumentRepository)
    {
        _licenseDocumentRepository = licenseDocumentRepository;
    }

    public async Task<Result<LicenseDocumentDto>> Handle(GetLicenseDocumentRequest request, CancellationToken ct)
    {
        var document = await _licenseDocumentRepository.GetByIdAsync(new LicenseDocumentId(request.Id), ct);
        if (document is null)
        {
            return Result<LicenseDocumentDto>.Failure(new NotFoundError("Document not found"));
        }

        var dto = new LicenseDocumentDto(
            Id: document.Id.Value,
            LicenseId: document.LicenseId.Value,
            DocumentType: document.DocumentType,
            FileUrl: document.FileUrl,
            UploadedAt: document.UploadedAt,
            CreatedAt: document.CreatedAt
        );

        return Result<LicenseDocumentDto>.Success(dto);
    }
}
