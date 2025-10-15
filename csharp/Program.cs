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
    description = "Generates comprehensive, marketing-ready product descriptions based on product name, part number, and attributes. Creates structured content with overview, features, specifications, and applications.",
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
                        required = false,
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
            string.IsNullOrWhiteSpace(request.PartNumber))
        {
            return Results.BadRequest(new ToolResponse
            {
                Success = false,
                Error = "Missing required parameters",
                Details = "Both productName and partNumber are required"
            });
        }

        // Execute tool logic (OpalTool.Execute() equivalent)
        var description = GenerateProductDescription(
            request.ProductName,
            request.PartNumber,
            request.Attributes ?? new List<string>()
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
                attributeCount = request.Attributes?.Count ?? 0
            },
            Content = description,
            Metadata = new
            {
                tool = toolMetadata.name,
                version = toolMetadata.version,
                productName = request.ProductName,
                partNumber = request.PartNumber,
                attributeCount = request.Attributes?.Count ?? 0
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
    var sb = new StringBuilder();

    // Header
    sb.AppendLine("# Product Description");
    sb.AppendLine();
    sb.AppendLine($"## {productName}");
    sb.AppendLine();
    sb.AppendLine($"**Part Number:** `{partNumber}`\n");
    sb.AppendLine("---\n");

    // Overview
    sb.AppendLine("## Overview\n");
    sb.AppendLine(GenerateOverview(productName, attributes));
    sb.AppendLine();

    // Key Features
    sb.AppendLine("## Key Features\n");
    sb.AppendLine(GenerateKeyFeatures(productName, attributes));
    sb.AppendLine();

    // Technical Specifications
    sb.AppendLine("## Technical Specifications\n");
    sb.AppendLine($"**Product Name:** {productName}  ");
    sb.AppendLine($"**Part Number:** {partNumber}\n");

    // Product Attributes
    sb.AppendLine("## Product Attributes\n");
    if (attributes.Any())
    {
        foreach (var attr in attributes)
        {
            sb.AppendLine($"- {attr}");
        }
    }
    else
    {
        sb.AppendLine("- No additional attributes specified");
    }
    sb.AppendLine("\n---\n");

    // Why Choose
    sb.AppendLine("## Why Choose This Product?\n");
    sb.AppendLine(GenerateWhyChoose(productName, attributes));
    sb.AppendLine();

    // Applications
    sb.AppendLine("## Product Applications\n");
    sb.AppendLine(GenerateApplications(attributes));
    sb.AppendLine("\n---\n");

    // Footer
    sb.AppendLine($"*Generated product description for {productName} (Part #: {partNumber})*");

    return sb.ToString();
}

static string GenerateOverview(string productName, List<string> attributes)
{
    var overview = $"The **{productName}** is a premium product designed to deliver exceptional performance and reliability. ";
    
    if (attributes.Any())
    {
        overview += $"This product features {attributes.Count} key attribute{(attributes.Count > 1 ? "s" : "")} that make it stand out in its category. ";
    }
    
    overview += $"Engineered with precision and built to last, the {productName} represents the perfect balance of quality, functionality, and value.";
    
    return overview;
}

static string GenerateKeyFeatures(string productName, List<string> attributes)
{
    var features = new List<string>();
    
    // Add attribute-based features
    if (attributes.Any())
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            features.Add($"{i + 1}. **{attributes[i]}** - Carefully selected to enhance product performance");
        }
    }
    else
    {
        // Default features when no attributes provided
        features.Add("1. **High Quality Construction** - Built to last with premium materials");
        features.Add("2. **Reliable Performance** - Consistent results you can count on");
        features.Add("3. **Easy Integration** - Seamlessly fits into your workflow");
    }
    
    // Add standard features
    features.Add($"{features.Count + 1}. **Quality Assurance** - Every {productName} undergoes rigorous testing");
    features.Add($"{features.Count + 1}. **Customer Support** - Backed by comprehensive warranty and support");
    
    return string.Join("\n", features);
}

static string GenerateWhyChoose(string productName, List<string> attributes)
{
    var reasons = new List<string>
    {
        $"The **{productName}** has been carefully designed to meet the highest standards of quality and performance.",
        "",
        "**Benefits include:**",
        "- Superior quality and durability",
        "- Competitive pricing and value",
        "- Trusted by professionals worldwide",
        "- Comprehensive documentation and support"
    };
    
    if (attributes.Count > 3)
    {
        reasons.Add("- Extensive feature set with multiple configuration options");
    }
    
    return string.Join("\n", reasons);
}

static string GenerateApplications(List<string> attributes)
{
    var applications = new List<string>
    {
        "This product is ideal for:",
        "- Commercial applications",
        "- Industrial settings",
        "- Professional use cases",
        "- Integration with existing systems",
        ""
    };

    applications.Add(attributes.Any()
        ? $"With its {attributes.Count} specialized attributes, this product adapts to various use cases and requirements."
        : "Versatile design allows for use across multiple industries and applications.");

    return string.Join("\n", applications);
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
    public List<string>? Attributes { get; set; }
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
