using Microsoft.EntityFrameworkCore;
using LicenseService.Persistence.Common.Abstractions;
using LicenseService.Persistence.Data;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;

namespace LicenseService.Persistence.Repositories;

/// <summary>
/// Repository for license persistence operations
/// </summary>
internal sealed class LicenseRepository : Repository<License, LicenseId>, ILicenseRepository
{
    public LicenseRepository(DataContext dataContext) 
        : base(dataContext)
    {
    }

    public async Task<IEnumerable<License>> GetByApplicantIdAsync(Guid applicantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<License>()
            .Where(l => l.ApplicantId == applicantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<License> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? applicantId = null,
        string? status = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<License> query = _dataContext.Set<License>()
            .Include(l => l.LicenseType);

        // Apply filters
        if (applicantId.HasValue)
        {
            query = query.Where(l => l.ApplicantId == applicantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<LicenseStatus>(status, true, out var statusEnum))
        {
            query = query.Where(l => l.Status == statusEnum);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "submittedat" => sortDescending ? query.OrderByDescending(l => l.SubmittedAt) : query.OrderBy(l => l.SubmittedAt),
            "approvedat" => sortDescending ? query.OrderByDescending(l => l.ApprovedAt) : query.OrderBy(l => l.ApprovedAt),
            "expirydate" => sortDescending ? query.OrderByDescending(l => l.ExpiryDate) : query.OrderBy(l => l.ExpiryDate),
            "status" => sortDescending ? query.OrderByDescending(l => l.Status) : query.OrderBy(l => l.Status),
            _ => sortDescending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
