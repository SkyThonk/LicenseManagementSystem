namespace LicenseManagement.Web.ViewModels.Shared;

/// <summary>
/// Alert/notification message view model
/// </summary>
public class AlertViewModel
{
    public string Message { get; set; } = string.Empty;
    public AlertType Type { get; set; } = AlertType.Info;
    public bool IsDismissible { get; set; } = true;
    public bool Dismissible { get; set; } = true;
    public bool AutoDismiss { get; set; } = false;
    public int AutoDismissDelay { get; set; } = 5000;
    public string? Title { get; set; }

    public static AlertViewModel Success(string message, string? title = null) =>
        new() { Message = message, Type = AlertType.Success, Title = title };

    public static AlertViewModel Error(string message, string? title = null) =>
        new() { Message = message, Type = AlertType.Danger, Title = title };

    public static AlertViewModel Warning(string message, string? title = null) =>
        new() { Message = message, Type = AlertType.Warning, Title = title };

    public static AlertViewModel Info(string message, string? title = null) =>
        new() { Message = message, Type = AlertType.Info, Title = title };
}

public enum AlertType
{
    Success,
    Warning,
    Danger,
    Info
}
