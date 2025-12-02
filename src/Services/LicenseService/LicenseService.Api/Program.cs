using LicenseService.Api;
using LicenseService.Application;
using LicenseService.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using LicenseService.Persistence;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOpenApi();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOrigin",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
    });

    /*
    Dependency Injection configuration for services in Api, Application, 
    Infrastructure, and Persistence projects. Each of them contains DI
    for different services, and we're adding those services to this
    ASP.NET web application project. */
    builder.Services
        .AddApi(builder.Configuration)
        .AddApplication()
        .AddPersistence(builder)
        .AddInfrastructure(builder);
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Enable CORS early so preflight isn't blocked by auth
    app.UseCors("AllowAnyOrigin");

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();

    // Middleware for Authorization (e.g., Jwt).
    app.UseAuthorization();

    app.UseExceptionHandler(exceptionApp =>
    {
        exceptionApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            var problem = problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred",
                detail: exception?.Message
            );

            context.Response.StatusCode = problem.Status ?? 500;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        });
    });

    // Middleware to automatically register all Controller APIs with the application.
    app.MapControllers();

    // Redirect root URL to swagger
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });

    app.Run();
}
