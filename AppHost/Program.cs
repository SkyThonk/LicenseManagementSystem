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

// LicenseService Service connection string
var licenseServiceConnectionString = builder.Configuration["LicenseService:ConnectionStrings:SQL"];

// DocumentService Service connection string
var documentServiceConnectionString = builder.Configuration["DocumentService:ConnectionStrings:SQL"];

// NotificationService Service connection string
var notificationServiceConnectionString = builder.Configuration["NotificationService:ConnectionStrings:SQL"];

// PaymentService Service connection string
var paymentServiceConnectionString = builder.Configuration["PaymentService:ConnectionStrings:SQL"];

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

// Register LicenseService microservice with centralized configuration and fixed ports
var licenseServiceApi = builder.AddProject<Projects.LicenseService_Api>("LicenseService")
    .WithHttpEndpoint(port: 5003, name: "licenseservice-http");

if (!builder.Environment.IsDevelopment())
{
    licenseServiceApi = licenseServiceApi.WithHttpsEndpoint(port: 7003, name: "licenseservice-https");
}
licenseServiceApi = licenseServiceApi
    .WithEnvironment("ConnectionStrings__SQL", licenseServiceConnectionString)
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

// Register DocumentService microservice with centralized configuration and fixed ports
var documentServiceApi = builder.AddProject<Projects.DocumentService_Api>("DocumentService")
    .WithHttpEndpoint(port: 5004, name: "documentservice-http");

if (!builder.Environment.IsDevelopment())
{
    documentServiceApi = documentServiceApi.WithHttpsEndpoint(port: 7004, name: "documentservice-https");
}
documentServiceApi = documentServiceApi
    .WithEnvironment("ConnectionStrings__SQL", documentServiceConnectionString)
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

// Register NotificationService microservice with centralized configuration and fixed ports
var notificationServiceApi = builder.AddProject<Projects.NotificationService_Api>("NotificationService")
    .WithHttpEndpoint(port: 5005, name: "notificationservice-http");

if (!builder.Environment.IsDevelopment())
{
    notificationServiceApi = notificationServiceApi.WithHttpsEndpoint(port: 7005, name: "notificationservice-https");
}
notificationServiceApi = notificationServiceApi
    .WithEnvironment("ConnectionStrings__SQL", notificationServiceConnectionString)
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

// Register PaymentService microservice with centralized configuration and fixed ports
var paymentServiceApi = builder.AddProject<Projects.PaymentService_Api>("PaymentService")
    .WithHttpEndpoint(port: 5006, name: "paymentservice-http");

if (!builder.Environment.IsDevelopment())
{
    paymentServiceApi = paymentServiceApi.WithHttpsEndpoint(port: 7006, name: "paymentservice-https");
}
paymentServiceApi = paymentServiceApi
    .WithEnvironment("ConnectionStrings__SQL", paymentServiceConnectionString)
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

// Register Web UI project with reference to TenantService for API calls
var webApp = builder.AddProject<Projects.LicenseManagement_Web>("WebApp")
    .WithHttpEndpoint(port: 5000, name: "webapp-http")
    .WithReference(tenantServiceApi)
    .WithReference(licenseServiceApi)
    .WithReference(documentServiceApi)
    .WithReference(notificationServiceApi)
    .WithReference(paymentServiceApi)
    .WithEnvironment("TenantServiceUrl", "http://localhost:5002")
    .WithEnvironment("LicenseServiceUrl", "http://localhost:5003")
    .WithEnvironment("DocumentServiceUrl", "http://localhost:5004")
    .WithEnvironment("NotificationServiceUrl", "http://localhost:5005")
    .WithEnvironment("PaymentServiceUrl", "http://localhost:5006");

if (!builder.Environment.IsDevelopment())
{
    webApp = webApp.WithHttpsEndpoint(port: 7000, name: "webapp-https");
}

builder.Build().Run();
