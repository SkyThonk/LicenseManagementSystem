namespace LicenseService.Domain.Licenses;

public record LicenseId(Guid Value)
{
    public static implicit operator Guid(LicenseId id) => id.Value;
    public static implicit operator LicenseId(Guid value) => new(value);
}
