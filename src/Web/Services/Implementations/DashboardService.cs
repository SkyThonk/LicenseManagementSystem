using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Dashboard;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IApiService _apiService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IApiService apiService, ILogger<DashboardService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<DashboardIndexViewModel> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Call actual API endpoints
        // For now, return mock data for demonstration
        await Task.CompletedTask;
        
        var viewModel = new DashboardIndexViewModel
        {
            Stats = new List<StatCardViewModel>
            {
                new StatCardViewModel
                {
                    Title = "Total Tenants",
                    Value = "156",
                    Icon = "fas fa-building",
                    IconColor = "primary",
                    ChangePercentage = "+8.3%",
                    IsPositiveChange = true,
                    Period = "vs last month"
                },
                new StatCardViewModel
                {
                    Title = "Active Licenses",
                    Value = "1,234",
                    Icon = "fas fa-key",
                    IconColor = "success",
                    ChangePercentage = "+3.8%",
                    IsPositiveChange = true,
                    Period = "vs last month"
                },
                new StatCardViewModel
                {
                    Title = "Monthly Revenue",
                    Value = "$48,250",
                    Icon = "fas fa-dollar-sign",
                    IconColor = "info",
                    ChangePercentage = "+12.1%",
                    IsPositiveChange = true,
                    Period = "vs last month"
                },
                new StatCardViewModel
                {
                    Title = "Expiring Soon",
                    Value = "23",
                    Icon = "fas fa-exclamation-triangle",
                    IconColor = "warning",
                    ChangePercentage = "+27.8%",
                    IsPositiveChange = false,
                    Period = "in next 30 days"
                }
            },
            RecentLicenses = new List<RecentLicenseItem>
            {
                new RecentLicenseItem
                {
                    Id = Guid.NewGuid(),
                    TenantName = "Acme Corporation",
                    LicenseKey = "ACME-2024-XXXX-XXXX",
                    Type = "Enterprise",
                    Status = "Active",
                    ExpirationDate = DateTime.UtcNow.AddMonths(6)
                },
                new RecentLicenseItem
                {
                    Id = Guid.NewGuid(),
                    TenantName = "Tech Solutions Inc.",
                    LicenseKey = "TECH-2024-XXXX-XXXX",
                    Type = "Professional",
                    Status = "Active",
                    ExpirationDate = DateTime.UtcNow.AddMonths(3)
                },
                new RecentLicenseItem
                {
                    Id = Guid.NewGuid(),
                    TenantName = "Global Services Ltd.",
                    LicenseKey = "GLOB-2024-XXXX-XXXX",
                    Type = "Basic",
                    Status = "Expiring",
                    ExpirationDate = DateTime.UtcNow.AddDays(15)
                }
            },
            RecentPayments = new List<PaymentSummary>
            {
                new PaymentSummary(
                    Guid.NewGuid(),
                    "Acme Corporation",
                    299.00m,
                    "USD",
                    "Completed",
                    DateTime.UtcNow.AddDays(-1)
                ),
                new PaymentSummary(
                    Guid.NewGuid(),
                    "Tech Solutions Inc.",
                    199.00m,
                    "USD",
                    "Completed",
                    DateTime.UtcNow.AddDays(-2)
                ),
                new PaymentSummary(
                    Guid.NewGuid(),
                    "StartUp Co.",
                    99.00m,
                    "USD",
                    "Pending",
                    DateTime.UtcNow.AddDays(-3)
                )
            },
            RecentActivities = new List<RecentActivityItem>
            {
                new RecentActivityItem
                {
                    Type = "license_created",
                    Description = "New license created for Acme Corporation",
                    Timestamp = DateTime.UtcNow.AddMinutes(-30),
                    Icon = "fas fa-plus-circle",
                    IconColor = "success"
                },
                new RecentActivityItem
                {
                    Type = "payment_received",
                    Description = "Payment received from Tech Solutions Inc.",
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    Icon = "fas fa-dollar-sign",
                    IconColor = "info"
                },
                new RecentActivityItem
                {
                    Type = "tenant_registered",
                    Description = "New tenant registered: Global Services Ltd.",
                    Timestamp = DateTime.UtcNow.AddHours(-5),
                    Icon = "fas fa-user-plus",
                    IconColor = "primary"
                },
                new RecentActivityItem
                {
                    Type = "license_expiring",
                    Description = "License expiring soon for StartUp Co.",
                    Timestamp = DateTime.UtcNow.AddHours(-8),
                    Icon = "fas fa-exclamation-triangle",
                    IconColor = "warning"
                }
            },
            LicenseChartData = new ChartData
            {
                Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "New Licenses",
                        Data = new List<int> { 45, 52, 38, 65, 72, 89 },
                        BackgroundColor = "#6366f1"
                    },
                    new ChartDataset
                    {
                        Label = "Renewals",
                        Data = new List<int> { 28, 35, 42, 38, 45, 52 },
                        BackgroundColor = "#10b981"
                    }
                }
            },
            RevenueChartData = new ChartData
            {
                Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Revenue",
                        Data = new List<int> { 32500, 38200, 35800, 42100, 45600, 48250 },
                        BackgroundColor = "#6366f1"
                    }
                }
            }
        };

        return viewModel;
    }
}
