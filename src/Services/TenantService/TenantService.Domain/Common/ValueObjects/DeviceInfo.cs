using TenantService.Domain.Common.Exceptions;

namespace TenantService.Domain.Common.ValueObjects;

public record DeviceInfo(
    string AppVersion,
    string OsVersion,
    string Model,
    string Build)
{
    public static DeviceInfo Create(string appVersion, string osVersion, string model, string build)
    {
        if (string.IsNullOrWhiteSpace(appVersion))
            throw new DomainException("App version cannot be empty");
        if (string.IsNullOrWhiteSpace(osVersion))
            throw new DomainException("OS version cannot be empty");
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model cannot be empty");
        if (string.IsNullOrWhiteSpace(build))
            throw new DomainException("Build cannot be empty");

        return new DeviceInfo(appVersion, osVersion, model, build);
    }
}

