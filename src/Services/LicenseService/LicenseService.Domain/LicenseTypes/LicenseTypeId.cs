namespace LicenseService.Domain.LicenseTypes;

public record LicenseTypeId(Guid Value)
{
    public static implicit operator Guid(LicenseTypeId id) => id.Value;
    public static implicit operator LicenseTypeId(Guid value) => new(value);
}
