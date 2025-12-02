using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record Money(decimal Amount, string Currency)
{
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty");

        return new Money(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot add money with different currencies");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot subtract money with different currencies");
        return new Money(Amount - other.Amount, Currency);
    }
}

