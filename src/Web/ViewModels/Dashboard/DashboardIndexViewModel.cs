using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Dashboard;

/// <summary>
/// Dashboard index page view model
/// </summary>
public class DashboardIndexViewModel : BaseViewModel
{
    public List<StatCardViewModel> Stats { get; set; } = [];
    public List<RecentActivityItem> RecentActivities { get; set; } = [];
    public List<LicenseStatusSummary> LicenseStatusSummary { get; set; } = [];
    public List<PaymentSummary> RecentPayments { get; set; } = [];
    public List<RecentLicenseItem> RecentLicenses { get; set; } = [];
    public ChartData LicenseChartData { get; set; } = new();
    public ChartData RevenueChartData { get; set; } = new();
}

public class RecentActivityItem
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string IconColor { get; set; } = string.Empty;
}

public class RecentLicenseItem
{
    public Guid Id { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
}

public class RecentPaymentItem
{
    public Guid Id { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public record LicenseStatusSummary(
    string Status,
    int Count,
    string Color
);

public record PaymentSummary(
    Guid Id,
    string TenantName,
    decimal Amount,
    string Currency,
    string Status,
    DateTime Date
);

public class ChartData
{
    public List<string> Labels { get; set; } = [];
    public List<ChartDataset> Datasets { get; set; } = [];
}

public class ChartDataset
{
    public string Label { get; set; } = string.Empty;
    public List<int> Data { get; set; } = [];
    public string BackgroundColor { get; set; } = string.Empty;
}
