namespace LicenseService.Domain.Renewals;

public record RenewalId(Guid Value)
{
    public static implicit operator Guid(RenewalId id) => id.Value;
    public static implicit operator RenewalId(Guid value) => new(value);
}
