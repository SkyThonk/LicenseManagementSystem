using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// builder.AddOpenTelemetryCollector("otel");

// Get JWT settings from configuration commonly used across microservices
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var jwtExpiryMinutes = builder.Configuration["JwtSettings:ExpiryMinutes"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];


// UserManagement Service connection string
var userManagementConnectionString = builder.Configuration["UserManagement:ConnectionStrings:SQL"];

// TenantService Service connection string
var tenantServiceConnectionString = builder.Configuration["TenantService:ConnectionStrings:SQL"];

// Get Redis connection string for external Redis Cloud
var redisConnectionString = builder.Configuration["ConnectionStrings:redis"];

// Get Kafka configuration
var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
var kafkaSecurityProtocol = builder.Configuration["Kafka:SecurityProtocol"];
var kafkaSaslMechanism = builder.Configuration["Kafka:SaslMechanism"];
var kafkaSaslUsername = builder.Configuration["Kafka:SaslUsername"];
var kafkaSaslPassword = builder.Configuration["Kafka:SaslPassword"];



// Register TenantService microservice with centralized configuration and fixed ports
var tenantServiceApi = builder.AddProject<Projects.TenantService_Api>("TenantService")
    .WithHttpEndpoint(port: 5002, name: "tenantservice-http");

// For local development, avoid exposing the microservice over HTTPS.
// This prevents the Aspire host from listening on HTTPS for the service while in Development.
if (!builder.Environment.IsDevelopment())
{
    // In non-development environments (e.g., staging/production), enable HTTPS endpoint
    tenantServiceApi = tenantServiceApi.WithHttpsEndpoint(port: 7002, name: "tenantservice-https");
}
tenantServiceApi = tenantServiceApi
    .WithEnvironment("ConnectionStrings__SQL", tenantServiceConnectionString)
    .WithEnvironment("ConnectionStrings__redis", redisConnectionString)
    .WithEnvironment("Kafka__BootstrapServers", kafkaBootstrapServers)
    .WithEnvironment("Kafka__SecurityProtocol", kafkaSecurityProtocol)
    .WithEnvironment("Kafka__SaslMechanism", kafkaSaslMechanism)
    .WithEnvironment("Kafka__SaslUsername", kafkaSaslUsername)
    .WithEnvironment("Kafka__SaslPassword", kafkaSaslPassword)
    .WithEnvironment("JwtSettings__Secret", jwtSecret)
    .WithEnvironment("JwtSettings__ExpiryMinutes", jwtExpiryMinutes)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience);

builder.Build().Run();
