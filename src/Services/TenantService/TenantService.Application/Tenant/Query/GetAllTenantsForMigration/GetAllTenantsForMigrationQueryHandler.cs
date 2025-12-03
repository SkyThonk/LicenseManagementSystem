using TenantService.Contracts.Tenant.GetAllTenantsForMigration;
using TenantService.Application.Common.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using Common.Application.Result;

namespace TenantService.Application.Tenant.Query.GetAllTenantsForMigration;

/// <summary>
/// Handler for getting all tenants for database migration
/// </summary>
public class GetAllTenantsForMigrationQueryHandler : IQueryHandler<GetAllTenantsForMigrationRequest, GetAllTenantsForMigrationResponse>
{
    private readonly ITenantRepository _repo;

    public GetAllTenantsForMigrationQueryHandler(ITenantRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<GetAllTenantsForMigrationResponse>> Handle(GetAllTenantsForMigrationRequest request, CancellationToken ct)
    {
        var tenants = await _repo.GetAllActiveAsync(ct);

        var tenantDtos = tenants.Select(t => new TenantMigrationDto(
            t.Id.Value,
            t.Name,
            null // Connection string can be added later if per-tenant databases are needed
        ));

        return Result<GetAllTenantsForMigrationResponse>.Success(
            new GetAllTenantsForMigrationResponse(tenantDtos)
        );
    }
}
