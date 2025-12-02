using System.Text;
using TenantService.Api.Common;
using Common.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;


namespace TenantService.Api;

// Dependency Injection configuration for the Api layer.
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddControllers();
        
        services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Government Agency (Tenant) Management API", Version = "v1" });
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
                // Add custom header globally
               
                options.SupportNonNullableReferenceTypes();
            }
            );


        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Retrieve JWT settings from configuration
                var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };


                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse(); 

                        var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        var problem = problemDetailsFactory.CreateProblemDetails(
                            context.HttpContext,
                            statusCode: StatusCodes.Status401Unauthorized,
                            title: "Unauthorized",
                            detail: "The provided token is invalid or missing."
                        );

                        context.Response.StatusCode = problem.Status ?? 401;
                        context.Response.ContentType = "application/problem+json";
                        await context.Response.WriteAsJsonAsync(problem);
                    },
                    OnForbidden = async context =>
                    {
                        var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        var problem = problemDetailsFactory.CreateProblemDetails(
                            context.HttpContext,
                            statusCode: StatusCodes.Status403Forbidden,
                            title: "Forbidden",
                            detail: "You do not have permission to access this resource."
                        );

                        context.Response.StatusCode = problem.Status ?? 403;
                        context.Response.ContentType = "application/problem+json";
                        await context.Response.WriteAsJsonAsync(problem);
                    }
                };
            });


        services.AddAuthorization(options =>
        {
            options.AddPolicy("jwt", policy =>
                policy.Requirements.Add(new CustomTokenRequirement()));
        });

        services.AddSingleton<IAuthorizationHandler, CustomTokenHandler>();

        services.AddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();

        services.AddHttpContextAccessor();
        return services;
    }
}

