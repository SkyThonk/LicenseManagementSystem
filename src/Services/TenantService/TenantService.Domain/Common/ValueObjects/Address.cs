using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record Address(
    string AddressLineOne,
    string? AddressLineTwo,
    string City,
    string State,
    string? PostalCode = null,
    string? CountryCode = null)
{
    public static Address Create(
        string addressLineOne,
        string? addressLineTwo,
        string city,
        string state,
        string? postalCode = null,
        string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(addressLineOne))
            throw new DomainException("Address line one cannot be empty");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty");
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State cannot be empty");

        return new Address(addressLineOne, addressLineTwo, city, state, postalCode, countryCode);
    }
}

