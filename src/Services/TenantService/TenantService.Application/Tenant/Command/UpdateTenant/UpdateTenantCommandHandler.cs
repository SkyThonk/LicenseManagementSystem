using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.Tenant.UpdateTenant;
using TenantService.Domain.Tenant;
using TenantService.Domain.Common.ValueObjects;

namespace TenantService.Application.Tenant.Command.UpdateTenant;

/// <summary>
/// Handler for updating a government agency (tenant)
/// </summary>
public class UpdateTenantCommandHandler : ICommandHandler<UpdateTenantRequest, UpdateTenantResponse>
{
    private readonly ITenantRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateTenantCommandHandler(ITenantRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<UpdateTenantResponse>> Handle(UpdateTenantRequest request, CancellationToken ct)
    {
        var tenant = await _repo.GetByIdAsync(new TenantId(request.Id), ct);
        if (tenant is null)
        {
            return Result<UpdateTenantResponse>.Failure(new NotFoundError("Tenant not found"));
        }

        // Create address if provided
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.AddressLine) && 
            !string.IsNullOrWhiteSpace(request.City) && 
            !string.IsNullOrWhiteSpace(request.State))
        {
            address = Address.Create(
                request.AddressLine, 
                null, 
                request.City, 
                request.State,
                request.PostalCode,
                request.CountryCode);
        }

        // Create phone if provided
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.CountryCode) && 
            !string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            phone = PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
        }

        tenant.Update(
            name: request.Name,
            description: request.Description,
            address: address,
            phone: phone,
            email: request.Email,
            logo: request.Logo
        );

        _repo.Update(tenant);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateTenantResponse>.Success(new UpdateTenantResponse(
            tenant.Id.Value,
            tenant.Name,
            tenant.AgencyCode,
            tenant.IsActive,
            tenant.UpdatedAt
        ));
    }
}

