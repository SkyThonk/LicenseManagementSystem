using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record PlanDuration(string Value)
{
    public static PlanDuration Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Plan duration cannot be empty");

        return new PlanDuration(value);
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string[] parts = value.Trim().Split(' ');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out _)) return false;

        return parts[1].ToLower() is "day" or "days" or "month" or "months" or "year" or "years";
    }

    public TimeSpan ToTimeSpan()
    {
        // Simple parsing - could be enhanced based on format (e.g., "30 days", "1 month")
        if (Value.Contains("day"))
            return TimeSpan.FromDays(int.Parse(Value.Replace("days", "").Replace("day", "").Trim()));
        if (Value.Contains("month"))
            return TimeSpan.FromDays(int.Parse(Value.Replace("months", "").Replace("month", "").Trim()) * 30);
        if (Value.Contains("year"))
            return TimeSpan.FromDays(int.Parse(Value.Replace("years", "").Replace("year", "").Trim()) * 365);

        throw new DomainException("Invalid plan duration format");
    }
}

