using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Licenses;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class LicenseService : ILicenseService
{
    private readonly ILicenseApiService _apiService;
    private readonly ILogger<LicenseService> _logger;

    public LicenseService(ILicenseApiService apiService, ILogger<LicenseService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<LicenseListViewModel> GetLicensesAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status))
            {
                queryParams += $"&status={Uri.EscapeDataString(status)}";
            }

            var response = await _apiService.GetAsync<GetLicenseListApiResponse>(
                $"{ApiEndpoints.License.GetAll}{queryParams}",
                cancellationToken);

            if (response != null)
            {
                var items = response.Items.Select(MapToListItemViewModel).ToList();
                return new LicenseListViewModel
                {
                    Licenses = new PaginatedList<LicenseItemViewModel>(
                        items,
                        response.Pagination.TotalItems,
                        response.Pagination.Page,
                        response.Pagination.PageSize),
                    SearchQuery = search,
                    StatusFilter = status
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching licenses list");
        }

        return new LicenseListViewModel
        {
            Licenses = new PaginatedList<LicenseItemViewModel>([], 0, page, pageSize),
            SearchQuery = search,
            StatusFilter = status
        };
    }

    public async Task<LicenseDetailsViewModel?> GetLicenseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.License.GetById, id);
            var response = await _apiService.GetAsync<LicenseApiDto>(endpoint, cancellationToken);

            if (response != null)
            {
                return MapToDetailsViewModel(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching license {LicenseId}", id);
        }

        return null;
    }

    public async Task<bool> CreateLicenseAsync(LicenseFormViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CreateLicenseApiRequest
            {
                ApplicantId = Guid.NewGuid(), // In real scenario, this would come from the authenticated user
                LicenseTypeId = model.LicenseTypeId
            };

            var response = await _apiService.PostAsync<CreateLicenseApiRequest, CreateLicenseApiResponse>(
                ApiEndpoints.License.Create,
                request,
                cancellationToken);

            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating license");
            return false;
        }
    }

    public async Task<List<LicenseTypeViewModel>> GetLicenseTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching license types from {Endpoint}", ApiEndpoints.LicenseType.GetAll);
            
            var response = await _apiService.GetAsync<GetLicenseTypeListApiResponse>(
                ApiEndpoints.LicenseType.GetAll,
                cancellationToken);

            if (response != null && response.Items != null)
            {
                _logger.LogInformation("Received {Count} license types", response.Items.Count);
                return response.Items.Select(item => new LicenseTypeViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    FeeAmount = item.FeeAmount,
                    CreatedAt = item.CreatedAt
                }).ToList();
            }
            
            _logger.LogWarning("No license types received from API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching license types");
        }

        return [];
    }

    public async Task<bool> CreateLicenseTypeAsync(CreateLicenseTypeViewModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating license type: {Name} at {Endpoint}", model.Name, ApiEndpoints.LicenseType.Create);
            
            var request = new CreateLicenseTypeApiRequest
            {
                Name = model.Name,
                Description = model.Description,
                FeeAmount = model.FeeAmount
            };

            var response = await _apiService.PostAsync<CreateLicenseTypeApiRequest, CreateLicenseTypeApiResponse>(
                ApiEndpoints.LicenseType.Create,
                request,
                cancellationToken);

            if (response != null && response.Id != Guid.Empty)
            {
                _logger.LogInformation("License type created with Id: {Id}", response.Id);
                return true;
            }
            
            _logger.LogWarning("Failed to create license type - no valid response");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating license type");
            return false;
        }
    }

    public Task<bool> UpdateLicenseAsync(Guid id, LicenseFormViewModel model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating license: {Id}", id);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteLicenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DeactivateLicenseAsync(id, cancellationToken);
    }

    public async Task<bool> ActivateLicenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateLicenseStatusApiRequest { Status = "Approved" };
            var endpoint = $"{ApiEndpoints.License.GetAll}/{id}/approve";
            var response = await _apiService.PatchAsync<UpdateLicenseStatusApiRequest, UpdateLicenseStatusApiResponse>(
                endpoint, request, cancellationToken);
            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating license {LicenseId}", id);
            return false;
        }
    }

    public async Task<bool> DeactivateLicenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateLicenseStatusApiRequest { Status = "Rejected" };
            var endpoint = $"{ApiEndpoints.License.GetAll}/{id}/reject";
            var response = await _apiService.PatchAsync<UpdateLicenseStatusApiRequest, UpdateLicenseStatusApiResponse>(
                endpoint, request, cancellationToken);
            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating license {LicenseId}", id);
            return false;
        }
    }

    public async Task<bool> RenewLicenseAsync(Guid id, DateTime newExpirationDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateLicenseStatusApiRequest 
            { 
                Status = "Approved",
                ExpiryDate = newExpirationDate
            };
            var endpoint = $"{ApiEndpoints.License.GetAll}/{id}/approve?expiryDate={newExpirationDate:yyyy-MM-dd}";
            var response = await _apiService.PatchAsync<UpdateLicenseStatusApiRequest, UpdateLicenseStatusApiResponse>(
                endpoint, request, cancellationToken);
            return response?.Id != Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renewing license {LicenseId}", id);
            return false;
        }
    }

    private static LicenseItemViewModel MapToListItemViewModel(LicenseListItemApiDto dto)
    {
        return new LicenseItemViewModel
        {
            Id = dto.Id,
            LicenseNumber = $"LIC-{dto.Id.ToString()[..8].ToUpper()}",
            LicenseType = dto.LicenseTypeName ?? "Unknown",
            ApplicantName = dto.ApplicantId.ToString()[..8],
            Status = dto.Status,
            ApplicationDate = dto.SubmittedAt,
            ExpiryDate = dto.ExpiryDate,
            TenantName = "Current Tenant"
        };
    }

    private static LicenseDetailsViewModel MapToDetailsViewModel(LicenseApiDto dto)
    {
        return new LicenseDetailsViewModel
        {
            Id = dto.Id,
            LicenseNumber = $"LIC-{dto.Id.ToString()[..8].ToUpper()}",
            LicenseType = dto.LicenseTypeName ?? "Unknown",
            Status = dto.Status,
            ApplicationDate = dto.SubmittedAt,
            ApprovalDate = dto.ApprovedAt,
            ExpiryDate = dto.ExpiryDate,
            TenantName = "Current Tenant",
            TenantId = Guid.Empty,
            Applicant = new ApplicantViewModel
            {
                Id = dto.ApplicantId,
                FullName = "Applicant",
                Email = "applicant@example.com",
                Phone = "",
                Address = ""
            },
            Documents = new List<LicenseDocumentViewModel>(),
            Payments = new List<LicensePaymentViewModel>(),
            WorkflowHistory = new List<WorkflowHistoryItem>
            {
                new WorkflowHistoryItem("Created", "System", dto.CreatedAt, "License application submitted"),
            }
        };
    }
}

#region API DTOs

internal class GetLicenseListApiResponse
{
    public LicensePaginationDto Pagination { get; set; } = new();
    public List<LicenseListItemApiDto> Items { get; set; } = [];
}

internal class LicensePaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

internal class LicenseListItemApiDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid LicenseTypeId { get; set; }
    public string? LicenseTypeName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

internal class LicenseApiDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid LicenseTypeId { get; set; }
    public string? LicenseTypeName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

internal class CreateLicenseApiRequest
{
    public Guid ApplicantId { get; set; }
    public Guid LicenseTypeId { get; set; }
}

internal class CreateLicenseApiResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

internal class UpdateLicenseStatusApiRequest
{
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

internal class UpdateLicenseStatusApiResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// License Type DTOs
internal class GetLicenseTypeListApiResponse
{
    public LicensePaginationDto Pagination { get; set; } = new();
    public List<LicenseTypeListItemApiDto> Items { get; set; } = [];
}

internal class LicenseTypeListItemApiDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal FeeAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

internal class CreateLicenseTypeApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal FeeAmount { get; set; }
}

internal class CreateLicenseTypeApiResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal FeeAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion
