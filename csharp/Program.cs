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
    description = "Generates natural, AI-like product descriptions (up to 500 characters) dynamically based on any product attributes.",
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
    // Parse attributes into a map for easier access
    var attrMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var attrList = new List<(string key, string value, string original)>();
    
    foreach (var attr in attributes)
    {
        var colonIndex = attr.IndexOf(':');
        if (colonIndex > 0)
        {
            var key = attr.Substring(0, colonIndex).Trim().ToLower();
            var value = attr.Substring(colonIndex + 1).Trim();
            attrMap[key] = value;
            attrList.Add((key, value, attr));
        }
        else
        {
            attrList.Add((attr.ToLower(), attr, attr));
        }
    }

    // Start with engaging introduction
    var description = $"The {productName} (Part# {partNumber}) ";
    
    // Add opening statement - check for power/voltage/cordless to make it dynamic
    var hasPower = attrMap.ContainsKey("battery voltage (v)") || 
                   attrMap.ContainsKey("voltage") || 
                   attrMap.ContainsKey("power");
    var isCordless = attrMap.TryGetValue("cordless / corded", out var cordlessValue) && 
                     cordlessValue.ToLower() == "cordless";
    
    if (hasPower && isCordless)
    {
        description += "delivers powerful cordless performance. ";
    }
    else if (hasPower)
    {
        description += "offers reliable powered performance. ";
    }
    else
    {
        description += "provides professional-grade quality. ";
    }
    
    // Build feature highlights from meaningful attributes with context
    // Prioritize attributes with key information (capacity, cartridge type, etc.)
    var priorityKeys = new[] { "capacity", "cartridge type", "weight", "dimensions", "material" };
    var meaningfulAttrs = attrList
        .Where(a => !a.key.Contains("cs_") && 
                    a.key != "brand" &&
                    a.key != "cordless / corded" &&
                    a.value.ToLower() != "yes" && 
                    a.value.ToLower() != "no" &&
                    a.value.Length > 2)
        .OrderBy(a => priorityKeys.Any(k => a.key.Contains(k)) ? 0 : 1)
        .Take(3)
        .ToList();
    
    if (meaningfulAttrs.Any())
    {
        var features = string.Join(", ", meaningfulAttrs.Select(a => {
            // For better readability, include key for context if value doesn't already contain it
            if (a.value.Length < 15 && !a.value.ToLower().Contains(a.key.Split(' ')[0]))
            {
                return $"{a.key}: {a.value}";
            }
            return a.value;
        }));
        description += $"Features include {features}. ";
    }
    
    // Add brand statement if available
    if (attrMap.TryGetValue("brand", out var brand))
    {
        description += $"Built with {brand} quality and reliability. ";
    }
    
    // Add warranty if available
    if (attrMap.TryGetValue("cs_manufacturer_warranty", out var warranty))
    {
        var warrantySimple = warranty.Replace(" limited warranty", "").Split('/')[0];
        description += $"Backed by {warrantySimple} warranty. ";
    }
    else
    {
        description += "Designed for demanding applications. ";
    }
    
    // Ensure it stays under 500 characters
    if (description.Length > 500)
    {
        description = description.Substring(0, 497) + "...";
    }
    
    return description.Trim();
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
