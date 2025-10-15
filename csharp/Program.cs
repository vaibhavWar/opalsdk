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

// Configure the HTTP request pipeline
app.UseCors();
app.UseAuthorization();

app.MapControllers();

// Map Opal Tools endpoints (handles /discovery and execution automatically)
app.MapOpalTools();

app.Run();
