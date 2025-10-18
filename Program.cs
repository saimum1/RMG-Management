
using DotNetWbapi.Data;
using DotNetWbapi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using OfficeOpenXml;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


// Configure controllers for API endpoints
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to handle reference loops and formatting
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DotNetWbapi",
        Version = "v1",
        Description = "API for managing categories and delivery challans"
    });
});

// Configure HttpClient for external API calls if needed
builder.Services.AddHttpClient();

// Register database context with SQL Server

// Load environment variable safely
var useServer = bool.Parse(Environment.GetEnvironmentVariable("USE_MYSQL") ?? "false");
string connectionString = useServer
    ? Environment.GetEnvironmentVariable("SERVER_DB_CONNECTION") ?? ""
    : $"Server={Environment.GetEnvironmentVariable("LOCAL_DB_SERVER")};Database={Environment.GetEnvironmentVariable("LOCAL_DB_NAME")};User Id={Environment.GetEnvironmentVariable("LOCAL_DB_USER")};Password={Environment.GetEnvironmentVariable("LOCAL_DB_PASSWORD")};TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDBContext>(options =>
{
    if (useServer)
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    else
    {
        options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    }
});

// builder.Services.AddDbContext<AppDBContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
//            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())); 

// builder.Services.AddDbContext<AppDBContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//     ));

// Register application services

// builder.Services.AddScoped<DeliveryChallanService>(); 
builder.Services.AddScoped<GatepassService>();
builder.Services.AddScoped<DeliveryChallanServiceCreation>();
// builder.Services.AddScoped<PendingDeliveryItemServiceCreation>();
builder.Services.AddScoped<TruckService>();
builder.Services.AddScoped<DeclarationSettingService>();
builder.Services.AddScoped<PackagingListService>();
builder.Services.AddScoped<TemplateService>();
// Configure CORS to allow requests from the frontend (http://127.0.0.1:5500)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5500", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500","http://127.0.0.1:5501","https://fanciful-paprenjak-43fd9b.netlify.app")
               .AllowAnyMethod() // Supports GET, POST, DELETE, OPTIONS, etc.
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable developer exception page in development for detailed error information
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // app.UseSwagger();
    // app.UseSwaggerUI(c =>
    // {
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNetWbapi v1");
    //     c.RoutePrefix = string.Empty; // Serve Swagger UI at root (/)
    // });
}

// Apply CORS policy before other middleware to handle preflight OPTIONS requests
app.UseCors("AllowLocalhost5500");

// Serve static files (e.g., HTML, CSS, JS) if needed
app.UseStaticFiles();

// Enable routing for controllers 
app.UseRouting();

// Disable HTTPS redirection in development to avoid scheme mismatch
// app.UseHttpsRedirection(); // Commented out to prevent HTTPS issues in development

// Enable authorization (if used in future)
app.UseAuthorization();

// Map a simple health check endpoint
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));  
});


// Map API controllers
app.MapControllers();

app.Run();