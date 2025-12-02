using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Contracts.Tenant.BlockTenant;
using TenantService.Domain.Tenant;

namespace TenantService.Application.Tenant.Command.BlockTenant;

/// <summary>
/// Handler for activating/deactivating a government agency
/// </summary>
public class BlockTenantCommandHandler : ICommandHandler<BlockTenantRequest, BlockTenantResponse>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BlockTenantCommandHandler(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BlockTenantResponse>> Handle(BlockTenantRequest request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(new TenantId(request.TenantId), ct);
        if (tenant is null)
        {
            return Result<BlockTenantResponse>.Failure(new NotFoundError("Tenant not found"));
        }

        if (request.Activate)
        {
            if (tenant.IsActive)
            {
                return Result<BlockTenantResponse>.Success(new BlockTenantResponse(true, $"Agency '{tenant.Name}' is already active", tenant.IsActive));
            }
            tenant.Activate();
        }
        else
        {
            if (!tenant.IsActive)
            {
                return Result<BlockTenantResponse>.Success(new BlockTenantResponse(true, $"Agency '{tenant.Name}' is already inactive", tenant.IsActive));
            }
            tenant.Deactivate();
        }

        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(ct);

        var message = request.Activate 
            ? $"Agency '{tenant.Name}' has been activated" 
            : $"Agency '{tenant.Name}' has been deactivated";
        return Result<BlockTenantResponse>.Success(new BlockTenantResponse(true, message, tenant.IsActive));
    }
}

