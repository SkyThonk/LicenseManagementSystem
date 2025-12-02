using System.Text;
using Common.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LicenseService.Api;

// Dependency Injection configuration for the Api layer.
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddControllers();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "License Management API", Version = "v1" });
            // Security Definitions for JWT Bearer token.
            var securitySchema = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            options.AddSecurityDefinition("Bearer", securitySchema);

            // Security Requirement for Bearer token.
            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            };
            options.AddSecurityRequirement(securityRequirement);

            options.SupportNonNullableReferenceTypes();
        });

        // JWT Authentication Configuration
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        
        if (jwtSettings != null)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });
        }

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // Add ProblemDetails support
        services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();

        return services;
    }
}
