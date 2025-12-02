using TenantService.Contracts.Tenant.GetTenantProfile;
using TenantService.Domain.Tenant;
using TenantService.Contracts.Common.Dto;

namespace TenantService.Application.Tenant.Query.GetTenantProfile;

/// <summary>
/// Handler for getting government agency (tenant) profile
/// </summary>
public class GetTenantProfileQueryHandler : IQueryHandler<GetTenantProfileRequest, TenantProfileDto>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantProfileQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<TenantProfileDto>> Handle(GetTenantProfileRequest request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(new TenantId(request.TenantId), ct);
        if (tenant is null)
        {
            return Result<TenantProfileDto>.Failure(new NotFoundError("Tenant not found"));
        }

        // Map address DTO
        var addressDto = tenant.Address is not null ? new AddressDto(
            tenant.Address.AddressLineOne,
            tenant.Address.AddressLineTwo,
            tenant.Address.City,
            tenant.Address.State
        ) : null;

        // Map phone DTO
        var phoneDto = tenant.Phone is not null ? new PhoneDto(
            tenant.Phone.CountryCode,
            tenant.Phone.Number,
            tenant.Phone.FullNumber
        ) : null;

        var dto = new TenantProfileDto(
            Id: tenant.Id.Value,
            Name: tenant.Name,
            AgencyCode: tenant.AgencyCode,
            Description: tenant.Description,
            Email: tenant.Email,
            Logo: tenant.Logo,
            IsActive: tenant.IsActive,
            CreatedAt: tenant.CreatedAt,
            UpdatedAt: tenant.UpdatedAt,
            Address: addressDto,
            Phone: phoneDto
        );

        return Result<TenantProfileDto>.Success(dto);
    }
}

