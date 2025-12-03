using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Contracts.Tenant.RegisterTenant;
using TenantService.Domain.User;
using TenantService.Domain.Common.ValueObjects;
using DomainUser = TenantService.Domain.User.User;

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

        // Create the admin user for the tenant (with Admin role - can manage users but not tenants)
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = DomainUser.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            Role.Admin,
            tenant.Id
        );

        _userRepo.Add(user);

        await _uow.SaveChangesAsync(ct);

        // TenantCreatedEvent is raised by the Tenant entity and will be handled
        // by TenantCreatedEventHandler via Wolverine to publish to Kafka

        return Result<RegisterTenantResponse>.Success(new RegisterTenantResponse(
            tenant.Id.Value,
            tenant.Name,
            tenant.AgencyCode,
            tenant.CreatedAt
        ));
    }
}

