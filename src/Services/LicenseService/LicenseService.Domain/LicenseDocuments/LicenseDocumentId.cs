namespace LicenseService.Domain.LicenseDocuments;

public record LicenseDocumentId(Guid Value)
{
    public static implicit operator Guid(LicenseDocumentId id) => id.Value;
    public static implicit operator LicenseDocumentId(Guid value) => new(value);
}
