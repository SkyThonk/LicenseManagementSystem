namespace LicenseService.Domain.LicenseStatusHistory;

public record LicenseStatusHistoryId(Guid Value)
{
    public static implicit operator Guid(LicenseStatusHistoryId id) => id.Value;
    public static implicit operator LicenseStatusHistoryId(Guid value) => new(value);
}
