namespace TenantService.Contracts.Common.Dto;

public record AddressDto(
    string AddressLineOne,
    string? AddressLineTwo,
    string City,
    string State
);
