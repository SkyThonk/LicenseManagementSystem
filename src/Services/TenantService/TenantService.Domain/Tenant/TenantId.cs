namespace TenantService.Domain.Tenant;

public record TenantId(Guid Value)
{
    public static implicit operator Guid(TenantId id) => id.Value;
    public static implicit operator TenantId(Guid value) => new(value);
};

