/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * C# Implementation using Optimizely Opal Tools SDK Pattern
 * 
 * This implements the OpalTool pattern for .NET applications
 */

using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for Optimizely Opal
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// ============================================================================
// Optimizely Opal Tool Implementation
// Following the OpalTool SDK pattern for C#/.NET
// ============================================================================

// Tool metadata following Opal SDK pattern
var toolMetadata = new
{
    name = "product-description-generator",
    description = "Provides structured product information for Optimizely Opal AI to generate natural product descriptions up to 500 characters.",
    version = "1.0.0",
    sdkPattern = "optimizely-opal-tools-sdk"
};

// ============================================================================
// Discovery Endpoint (GET /discovery)
// Required by Optimizely Opal to discover tool capabilities
// Following Opal SDK discovery specification
// ============================================================================
app.MapGet("/discovery", () =>
{
    var discoveryResponse = new
    {
        functions = new[]
        {
            new
            {
                name = toolMetadata.name,
                description = toolMetadata.description,
                version = toolMetadata.version,
                parameters = new[]
                {
                    new
                    {
                        name = "productName",
                        type = "string",
                        description = "The name of the product",
                        required = true,
                        example = "Professional Drill Set"
                    },
                    new
                    {
                        name = "partNumber",
                        type = "string",
                        description = "The product part number or SKU",
                        required = true,
                        example = "DRL-2024-PRO"
                    },
                    new
                    {
                        name = "attributes",
                        type = "array",
                        description = "List of product attributes, features, or specifications (e.g., [\"Color: Blue\", \"Power: 20V\", \"Material: Steel\"])",
                        required = true,
                        example = new[] { "Color: Blue", "Power: 20V", "Weight: 3.5 lbs" },
                        items = new { type = "string" }
                    }
                },
                endpoint = "/",
                http_method = "POST",
                auth_requirements = Array.Empty<string>()
            }
        }
    };

    return Results.Ok(discoveryResponse);
})
.WithName("OpalDiscovery")
.WithOpenApi()
.WithTags("Optimizely Opal SDK");

// ============================================================================
// Health Check Endpoint (GET /)
// Provides tool status and SDK pattern identification
// ============================================================================
app.MapGet("/", () =>
{
    var healthResponse = new
    {
        status = "healthy",
        tool = toolMetadata.name,
        version = toolMetadata.version,
        description = toolMetadata.description,
        sdk_pattern = toolMetadata.sdkPattern,
        endpoints = new
        {
            discovery = "/discovery",
            health = "/",
            execute = "POST /"
        }
    };

    return Results.Ok(healthResponse);
})
.WithName("HealthCheck")
.WithOpenApi()
.WithTags("Optimizely Opal SDK");

// ============================================================================
// Tool Execution Endpoint (POST /)
// Main endpoint called by Optimizely Opal to execute the tool
// Implements the OpalTool.Execute() pattern
// ============================================================================
app.MapPost("/", async ([FromBody] ProductDescriptionRequest request) =>
{
    try
    {
        // Validate required parameters (OpalTool pattern)
        if (string.IsNullOrWhiteSpace(request.ProductName) || 
            string.IsNullOrWhiteSpace(request.PartNumber) ||
            request.Attributes == null)
        {
            return Results.BadRequest(new ToolResponse
            {
                Success = false,
                Error = "Missing required parameters",
                Details = "productName, partNumber, and attributes are required"
            });
        }

        // Validate attributes is not empty
        if (request.Attributes.Count == 0)
        {
            return Results.BadRequest(new ToolResponse
            {
                Success = false,
                Error = "Invalid attributes",
                Details = "attributes must contain at least one item"
            });
        }

        // Execute tool logic (OpalTool.Execute() equivalent)
        var description = GenerateProductDescription(
            request.ProductName,
            request.PartNumber,
            request.Attributes
        );

        // Return Opal SDK-compliant response with metadata
        return Results.Ok(new ToolResponse
        {
            Success = true,
            Result = new
            {
                content = description,
                productName = request.ProductName,
                partNumber = request.PartNumber,
                attributeCount = request.Attributes.Count
            },
            Content = description,
            Metadata = new
            {
                tool = toolMetadata.name,
                version = toolMetadata.version,
                productName = request.ProductName,
                partNumber = request.PartNumber,
                attributeCount = request.Attributes.Count
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new ToolResponse
        {
            Success = false,
            Error = "Failed to generate product description",
            Details = ex.Message
        });
    }
})
.WithName("ExecuteOpalTool")
.WithOpenApi()
.WithTags("Optimizely Opal SDK");

app.Run();

// ============================================================================
// Tool Implementation - Product Description Generation
// Following OpalTool pattern with private helper methods
// ============================================================================

static string GenerateProductDescription(
    string productName, 
    string partNumber, 
    List<string> attributes)
{
    // Return structured product information for Opal to generate description
    // Format: Product name, part number, and all attributes in a clean format
    var description = $"{productName}, Part# {partNumber}. ";
    
    // Add all attributes in a natural list format
    description += $"Key specifications: {string.Join(", ", attributes)}.";
    
    // Ensure it stays under 500 characters
    if (description.Length > 500)
    {
        description = description.Substring(0, 497) + "...";
    }
    
    return description;
}

// ============================================================================
// Data Transfer Objects (DTOs)
// Following Opal SDK request/response patterns
// ============================================================================

/// <summary>
/// Request model for product description generation
/// Follows Optimizely Opal SDK parameter pattern
/// </summary>
public class ProductDescriptionRequest
{
    public string ProductName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public List<string> Attributes { get; set; } = new List<string>();
}

/// <summary>
/// Response model following Optimizely Opal SDK pattern
/// Includes success indicator, result data, and metadata
/// </summary>
public class ToolResponse
{
    public bool Success { get; set; }
    public object? Result { get; set; }
    public string? Content { get; set; }
    public object? Metadata { get; set; }
    public string? Error { get; set; }
    public string? Details { get; set; }
}
