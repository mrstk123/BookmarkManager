using System.Text;
using System.Threading.RateLimiting;
using BookmarkManager.Application;
using BookmarkManager.Infrastructure;
using BookmarkManager.Infrastructure.Authentication;
using BookmarkManager.Infrastructure.Persistence;
using BookmarkManager.WebAPI.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Core MVC
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings is missing in configuration.");

builder.Services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Secret))
    });
builder.Services.AddAuthorization();

// CORS
var clientAppUrl = builder.Configuration["ClientAppUrl"] ?? "http://localhost:4200";
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngularClient", policy =>
        policy.WithOrigins(clientAppUrl)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()));

// Rate Limiting (built-in)
// Uses ASP.NET Core's built-in Microsoft.AspNetCore.RateLimiting
builder.Services.AddRateLimiter(options =>
{
    // Protects the unauthenticated check-email endpoint from enumeration attacks.
    // Allows 5 requests per minute per IP address; excess requests receive HTTP 429.
    options.AddPolicy("check-email-policy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0          // reject immediately, no queuing
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Application & Infrastructure Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// HTTP Pipeline
var app = builder.Build();

// Database Migrations (after Build so failures surface cleanly)
DatabaseSetup.RunMigrations(builder.Configuration);

// HTTP Pipeline - Exception handling must be FIRST to catch all errors
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseStaticFiles();
}

app.UseCors("AllowAngularClient");
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Angular SPA static files
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();