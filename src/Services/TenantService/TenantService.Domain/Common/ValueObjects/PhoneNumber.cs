using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record PhoneNumber(string CountryCode, string Number)
{
    public static PhoneNumber Create(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new DomainException("Country code cannot be empty");
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Phone number cannot be empty");

        // Basic validation for country code (e.g., +1, +91)
        if (!countryCode.StartsWith("+") || countryCode.Length < 2)
            throw new DomainException("Country code must start with + and be at least 2 characters");

        return new PhoneNumber(countryCode, number);
    }

    public string FullNumber => $"{CountryCode}{Number}";
}

