using Common.Application.Interfaces;
using Common.Application.Result;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Contracts.LicenseDocuments.GetLicenseDocumentList;
using LicenseService.Domain.Licenses;

namespace LicenseService.Application.LicenseDocuments.Queries.GetLicenseDocumentList;

/// <summary>
/// Handler for getting list of license documents by license ID
/// </summary>
public class GetLicenseDocumentListQueryHandler : IQueryHandler<GetLicenseDocumentListRequest, GetLicenseDocumentListResponse>
{
    private readonly ILicenseDocumentRepository _licenseDocumentRepository;

    public GetLicenseDocumentListQueryHandler(ILicenseDocumentRepository licenseDocumentRepository)
    {
        _licenseDocumentRepository = licenseDocumentRepository;
    }

    public async Task<Result<GetLicenseDocumentListResponse>> Handle(GetLicenseDocumentListRequest request, CancellationToken ct)
    {
        var documents = await _licenseDocumentRepository.GetByLicenseIdAsync(
            new LicenseId(request.LicenseId), ct);

        var documentItems = documents.Select(d => new LicenseDocumentListItemDto(
            Id: d.Id.Value,
            TenantId: d.TenantId,
            LicenseId: d.LicenseId.Value,
            DocumentType: d.DocumentType,
            FileUrl: d.FileUrl,
            UploadedAt: d.UploadedAt
        ));

        var response = new GetLicenseDocumentListResponse(documentItems);
        return Result<GetLicenseDocumentListResponse>.Success(response);
    }
}
