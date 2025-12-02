using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record FullName(string FirstName, string? MiddleName, string LastName)
{
    public static FullName Create(string firstName, string? middleName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        return new FullName(firstName, middleName, lastName);
    }

    public string DisplayName => $"{FirstName} {(MiddleName != null ? MiddleName + " " : "")}{LastName}";
}

