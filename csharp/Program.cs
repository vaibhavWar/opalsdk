using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Discovery endpoint - describes the tool to Optimizely Opal
app.MapGet("/discovery", () =>
{
    var discoveryResponse = new
    {
        functions = new[]
        {
            new
            {
                name = "product-description-generator",
                description = "Generates comprehensive product descriptions based on product name, part number, and attributes. Creates marketing-ready content with structured sections including overview, features, specifications, and applications.",
                parameters = new[]
                {
                    new
                    {
                        name = "productName",
                        type = "string",
                        description = "The name of the product",
                        required = true
                    },
                    new
                    {
                        name = "partNumber",
                        type = "string",
                        description = "The product part number or SKU",
                        required = true
                    },
                    new
                    {
                        name = "attributes",
                        type = "array",
                        description = "List of product attributes, features, or specifications (e.g., [\"Color: Blue\", \"Size: Large\", \"Material: Steel\"])",
                        required = false
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
.WithName("Discovery")
.WithOpenApi();

// Health check endpoint
app.MapGet("/", () =>
{
    var healthResponse = new
    {
        status = "healthy",
        tool = "product-description-generator",
        version = "1.0.0",
        description = "Optimizely Opal tool for generating product descriptions"
    };

    return Results.Ok(healthResponse);
})
.WithName("HealthCheck")
.WithOpenApi();

// Tool execution endpoint
app.MapPost("/", async ([FromBody] ProductDescriptionRequest request) =>
{
    try
    {
        // Validate required parameters
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

        // Generate the product description
        var description = GenerateProductDescription(
            request.ProductName,
            request.PartNumber,
            request.Attributes ?? new List<string>()
        );

        return Results.Ok(new ToolResponse
        {
            Success = true,
            Content = description
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
.WithName("GenerateProductDescription")
.WithOpenApi();

app.Run();

// Helper method to generate product description
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
    sb.AppendLine($"**Part Number:** `{partNumber}`");
    sb.AppendLine();
    sb.AppendLine("---");
    sb.AppendLine();

    // Overview
    sb.AppendLine("## Overview");
    sb.AppendLine();
    sb.AppendLine(GenerateOverview(productName, attributes));
    sb.AppendLine();

    // Key Features
    sb.AppendLine("## Key Features");
    sb.AppendLine();
    sb.AppendLine(GenerateKeyFeatures(productName, attributes));
    sb.AppendLine();

    // Technical Specifications
    sb.AppendLine("## Technical Specifications");
    sb.AppendLine();
    sb.AppendLine($"**Product Name:** {productName}  ");
    sb.AppendLine($"**Part Number:** {partNumber}");
    sb.AppendLine();

    // Product Attributes
    sb.AppendLine("## Product Attributes");
    sb.AppendLine();
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
    sb.AppendLine();
    sb.AppendLine("---");
    sb.AppendLine();

    // Why Choose
    sb.AppendLine("## Why Choose This Product?");
    sb.AppendLine();
    sb.AppendLine(GenerateWhyChoose(productName, attributes));
    sb.AppendLine();

    // Applications
    sb.AppendLine("## Product Applications");
    sb.AppendLine();
    sb.AppendLine(GenerateApplications(attributes));
    sb.AppendLine();
    sb.AppendLine("---");
    sb.AppendLine();

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

// Request and Response models
public class ProductDescriptionRequest
{
    public string ProductName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public List<string>? Attributes { get; set; }
}

public class ToolResponse
{
    public bool Success { get; set; }
    public string? Content { get; set; }
    public string? Error { get; set; }
    public string? Details { get; set; }
}

