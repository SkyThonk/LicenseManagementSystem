using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HTTP context accessor for session access in services
builder.Services.AddHttpContextAccessor();

// Configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = ".LicenseManagement.Session";
});

// Register Authentication service
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    var tenantServiceUrl = builder.Configuration["TenantServiceUrl"] ?? "http://localhost:5002";
    client.BaseAddress = new Uri(tenantServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register HTTP client for TenantService API calls (port 5002)
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TenantServiceUrl"] ?? "http://localhost:5002");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register HTTP client for LicenseService API calls (port 5003)
builder.Services.AddHttpClient<ILicenseApiService, LicenseApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LicenseServiceUrl"] ?? "http://localhost:5003");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register HTTP client for DocumentService API calls (port 5004)
builder.Services.AddHttpClient<IDocumentApiService, DocumentApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["DocumentServiceUrl"] ?? "http://localhost:5004");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register HTTP client for NotificationService API calls (port 5005)
builder.Services.AddHttpClient<INotificationApiService, NotificationApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NotificationServiceUrl"] ?? "http://localhost:5005");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register HTTP client for PaymentService API calls (port 5006)
builder.Services.AddHttpClient<IPaymentApiService, PaymentApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PaymentServiceUrl"] ?? "http://localhost:5006");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

// Route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
