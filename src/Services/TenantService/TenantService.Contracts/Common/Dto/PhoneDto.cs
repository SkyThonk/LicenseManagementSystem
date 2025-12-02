namespace TenantService.Contracts.Common.Dto;

public record PhoneDto(
    string CountryCode,
    string Number,
    string FullNumber
);
