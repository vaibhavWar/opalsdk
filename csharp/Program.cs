/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * C# Implementation using Optimizely.Opal.Tools SDK v0.4.0
 * 
 * This tool generates natural, AI-like product descriptions
 * based on product name, part number, and attributes.
 * 
 * Architecture:
 * - Clean separation of concerns
 * - Dependency Injection
 * - Service layer for business logic
 * - Models for data transfer
 * - Tools for Opal integration
 */

using Optimizely.Opal.Tools;
using ProductDescriptionGenerator.Services;
using ProductDescriptionGenerator.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddControllers();

// Configure CORS for Optimizely Opal integration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register application services
builder.Services.AddScoped<IDescriptionGenerationService, DescriptionGenerationService>();

// Add Optimizely Opal Tool Service
builder.Services.AddOpalToolService();

// Register the Product Description Generator Tool
builder.Services.AddOpalTool<ProductDescriptionGeneratorTool>();

var app = builder.Build();

// Add global error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception occurred");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Internal Server Error",
            message = ex.Message,
            details = ex.ToString()
        });
    }
});

// Configure the HTTP request pipeline
app.UseCors();
app.UseAuthorization();

app.MapControllers();

// Map Opal Tools endpoints (handles /discovery and execution automatically)
app.MapOpalTools();

// Log startup info
app.Logger.LogInformation("Product Description Generator started");
app.Logger.LogInformation("Discovery endpoint: /discovery");
app.Logger.LogInformation("Tool endpoint: /tools/product-description-generator");

app.Run();
