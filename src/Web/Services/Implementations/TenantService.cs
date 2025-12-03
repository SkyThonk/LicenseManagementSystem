using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Shared;
using LicenseManagement.Web.ViewModels.Tenants;

namespace LicenseManagement.Web.Services.Implementations;

public class TenantService : ITenantService
{
    private readonly IApiService _apiService;
    private readonly ILogger<TenantService> _logger;

    public TenantService(IApiService apiService, ILogger<TenantService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<TenantListViewModel> GetTenantsAsync(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search))
            {
                queryParams += $"&search={Uri.EscapeDataString(search)}";
            }

            var response = await _apiService.GetAsync<TenantListApiResponse>(
                $"{ApiEndpoints.Tenant.List}{queryParams}",
                cancellationToken);

            if (response != null)
            {
                var items = response.Items.Select(MapToListItemViewModel).ToList();
                return new TenantListViewModel
                {
                    Tenants = new PaginatedList<TenantListItemViewModel>(
                        items,
                        response.Pagination.TotalItems,
                        response.Pagination.Page,
                        response.Pagination.PageSize),
                    SearchTerm = search,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenants list");
        }

        return new TenantListViewModel
        {
            Tenants = new PaginatedList<TenantListItemViewModel>([], 0, page, pageSize),
            SearchTerm = search,
            CurrentPage = page,
            PageSize = pageSize
        };
    }

    public async Task<TenantDetailsViewModel?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiService.GetAsync<TenantProfileDto>(
                $"{ApiEndpoints.Tenant.Profile}?tenantId={id}",
                cancellationToken);

            if (response != null)
            {
                return MapToDetailsViewModel(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant {TenantId}", id);
        }

        return null;
    }

    public async Task<bool> CreateTenantAsync(TenantFormViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RegisterTenantApiRequest
            {
                Name = model.Name,
                AgencyCode = model.AgencyCode,
                Description = model.Description,
                AddressLine = model.AddressLine,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                CountryCode = model.CountryCode,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                FirstName = model.FirstName ?? "Admin",
                LastName = model.LastName ?? "User",
                Password = model.Password ?? "ChangeMe123!",
                Logo = model.Logo
            };

            var response = await _apiService.PostAsync<RegisterTenantApiRequest, RegisterTenantApiResponse>(
                ApiEndpoints.Tenant.Register,
                request,
                cancellationToken);

            return response?.TenantId != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            return false;
        }
    }

    public async Task<bool> UpdateTenantAsync(Guid id, TenantFormViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateTenantApiRequest
            {
                Name = model.Name,
                Description = model.Description,
                Email = model.Email,
                Logo = model.Logo,
                AddressLine = model.AddressLine,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                CountryCode = model.CountryCode,
                PhoneNumber = model.PhoneNumber
            };

            var endpoint = string.Format(ApiEndpoints.Tenant.Update, id);
            var response = await _apiService.PutAsync<UpdateTenantApiRequest, UpdateTenantApiResponse>(
                endpoint,
                request,
                cancellationToken);

            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant {TenantId}", id);
            return false;
        }
    }

    public Task<bool> DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Tenants are not deleted, only deactivated
        return DeactivateTenantAsync(id, cancellationToken);
    }

    public async Task<bool> ActivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Tenant.Activate, id);
            var response = await _apiService.PatchAsync<BlockTenantApiResponse>(endpoint, cancellationToken);
            return response?.TenantId != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating tenant {TenantId}", id);
            return false;
        }
    }

    public async Task<bool> DeactivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Tenant.Deactivate, id);
            var response = await _apiService.PatchAsync<BlockTenantApiResponse>(endpoint, cancellationToken);
            return response?.TenantId != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating tenant {TenantId}", id);
            return false;
        }
    }

    private static TenantListItemViewModel MapToListItemViewModel(TenantListItemDto dto)
    {
        return new TenantListItemViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            AgencyCode = dto.AgencyCode,
            Email = dto.Email,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt
        };
    }

    private static TenantDetailsViewModel MapToDetailsViewModel(TenantProfileDto dto)
    {
        return new TenantDetailsViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            AgencyCode = dto.AgencyCode,
            Description = dto.Description,
            Email = dto.Email,
            Logo = dto.Logo,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            AddressLineOne = dto.Address?.AddressLineOne,
            AddressLineTwo = dto.Address?.AddressLineTwo,
            City = dto.Address?.City,
            State = dto.Address?.State,
            PhoneCountryCode = dto.Phone?.CountryCode,
            PhoneNumber = dto.Phone?.Number,
            PhoneFullNumber = dto.Phone?.FullNumber
        };
    }
}

#region API DTOs

// Response wrapper
internal class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}

// Tenant List Response - matches GetTenantListResponse from API
internal class TenantListApiResponse
{
    public PaginationDto Pagination { get; set; } = new();
    public List<TenantListItemDto> Items { get; set; } = [];
}

internal class PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

internal class TenantListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgencyCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Tenant Profile Response - matches TenantProfileDto from API
internal class TenantProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgencyCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public AddressDto? Address { get; set; }
    public PhoneDto? Phone { get; set; }
}

internal class AddressDto
{
    public string? AddressLineOne { get; set; }
    public string? AddressLineTwo { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
}

internal class PhoneDto
{
    public string? CountryCode { get; set; }
    public string? Number { get; set; }
    public string? FullNumber { get; set; }
}

// Register Tenant Request
internal class RegisterTenantApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string AgencyCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Logo { get; set; }
}

internal class RegisterTenantApiResponse
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}

// Update Tenant Request
internal class UpdateTenantApiRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Logo { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
}

// Update Tenant Response
internal class UpdateTenantApiResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgencyCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Block/Activate/Deactivate Tenant Response
internal class BlockTenantApiResponse
{
    public Guid TenantId { get; set; }
    public bool IsActive { get; set; }
}

#endregion
