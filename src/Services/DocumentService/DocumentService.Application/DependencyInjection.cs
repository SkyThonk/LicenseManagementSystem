using Microsoft.Extensions.DependencyInjection;

namespace DocumentService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Wolverine handles command/query dispatching
        // Handlers are discovered automatically
        return services;
    }
}
