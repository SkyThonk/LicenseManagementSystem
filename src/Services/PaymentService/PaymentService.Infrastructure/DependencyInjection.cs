using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentService.Infrastructure.Authentication;

namespace PaymentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder builder
    )
    {
        // Configure Common infrastructure JwtSettings for authentication
        services.Configure<Common.Infrastructure.Authentication.JwtSettings>(
            builder.Configuration.GetSection(Common.Infrastructure.Authentication.JwtSettings.SectionName));

        // Register Common JWT token generator
        services.AddSingleton<Common.Application.Interfaces.Authentication.IJwtTokenGenerator,
            Common.Infrastructure.Authentication.JwtTokenGenerator>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        return services;
    }
}
