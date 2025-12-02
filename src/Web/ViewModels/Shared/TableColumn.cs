namespace LicenseManagement.Web.ViewModels.Shared;

/// <summary>
/// Table column definition for dynamic tables
/// </summary>
public class TableColumn
{
    public string PropertyName { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsSortable { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public string? Width { get; set; }
    public string? CssClass { get; set; }
    public ColumnType Type { get; set; } = ColumnType.Text;
    public string? Format { get; set; }
}

public enum ColumnType
{
    Text,
    Date,
    DateTime,
    Currency,
    Badge,
    Actions,
    Link,
    Boolean
}
