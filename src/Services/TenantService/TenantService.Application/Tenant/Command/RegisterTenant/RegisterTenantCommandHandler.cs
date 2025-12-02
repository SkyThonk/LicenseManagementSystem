using Common.Application.Result;
using Common.Application.Interfaces;
using TenantService.Application.Common.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Contracts.Tenant.RegisterTenant;
using TenantService.Domain.Tenant;
using TenantService.Domain.User;
using TenantService.Domain.Common.ValueObjects;

namespace TenantService.Application.Tenant.Command.RegisterTenant;

/// <summary>
/// Handler for registering a new government agency (tenant)
/// </summary>
public class RegisterTenantCommandHandler : ICommandHandler<RegisterTenantRequest, RegisterTenantResponse>
{
    private readonly ITenantRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _uow;

    public RegisterTenantCommandHandler(
        ITenantRepository repo,
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        IUnitOfWork uow)
    {
        _repo = repo;
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _uow = uow;
    }

    public async Task<Result<RegisterTenantResponse>> Handle(RegisterTenantRequest request, CancellationToken ct)
    {
        // Check if agency code already exists
        if (await _repo.ExistsByAgencyCodeAsync(request.AgencyCode, ct))
        {
            return Result<RegisterTenantResponse>.Failure(new ValidationError("Agency code already exists"));
        }

        // Check if user email already exists
        if (await _userRepo.GetByEmailAsync(request.Email, ct) is not null)
        {
            return Result<RegisterTenantResponse>.Failure(new ValidationError("Email already exists"));
        }

        // Create address value object
        var address = Address.Create(
            request.AddressLine,
            null,
            request.City,
            request.State,
            request.PostalCode,
            request.CountryCode
        );

        // Create phone number value object
        var phone = PhoneNumber.Create(
            request.CountryCode,
            request.PhoneNumber
        );

        // Create the tenant (government agency)
        var tenant = Domain.Tenant.Tenant.Create(
            name: request.Name,
            agencyCode: request.AgencyCode,
            address: address,
            phone: phone,
            email: request.Email,
            description: request.Description,
            logo: request.Logo
        );

        _repo.Add(tenant);

        // Create the admin user for the tenant
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(
            request.Email,
            passwordHash,
            null, // firstName
            null, // lastName
            Role.TenantAdmin,
            tenant.Id
        );

        _userRepo.Add(user);

        await _uow.SaveChangesAsync(ct);

        return Result<RegisterTenantResponse>.Success(new RegisterTenantResponse(
            tenant.Id.Value,
            tenant.Name,
            tenant.AgencyCode,
            tenant.CreatedAt
        ));
    }
}

