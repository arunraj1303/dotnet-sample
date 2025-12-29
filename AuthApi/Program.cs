using AuthApi.Extensions;
using AuthApi.Middleware;
using AuthApi.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database Configuration (MySQL/MariaDB)
builder.Services.AddDatabaseConfiguration();

// JWT Authentication Configuration
builder.Services.AddJwtAuthentication();

// Identity Core (without cookie authentication for API)
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register Application Services (Repositories & Business Logic)
builder.Services.AddApplicationServices();

// Swagger Documentation Configuration
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Add request logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Incoming request: {Method} {Path}, Auth header: {HasAuth}", 
        context.Request.Method, 
        context.Request.Path,
        context.Request.Headers.ContainsKey("Authorization") ? "Yes" : "No");
    await next();
    logger.LogInformation("Response status: {StatusCode}", context.Response.StatusCode);
});

// Global Exception Handling Middleware (Should be first)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
