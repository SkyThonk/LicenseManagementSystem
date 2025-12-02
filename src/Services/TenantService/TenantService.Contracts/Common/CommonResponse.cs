namespace TenantService.Contracts.Common;

public record CommonResponse(
    string Message,
    bool Status
);

public record CommonResponse<T>(
    string Message,
    bool Status,
    T Data
);

