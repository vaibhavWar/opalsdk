/**
 * Optimizely Opal Custom Tool: Product Description Generator
 * C# Implementation using Optimizely.Opal.Tools SDK v0.4.0
 * 
 * This tool generates natural, AI-like product descriptions
 * based on product name, part number, and attributes.
 * 
 * SDK Pattern:
 * - Uses [OpalTool] attribute for tool registration
 * - Uses [Description] for parameter descriptions
 * - Uses [Required] for required parameters
 * - Automatically handles discovery and execution endpoints
 */

using Optimizely.Opal.Tools;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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

// ============================================================================
// Parameter Model
// Defines the input parameters for the product description generator
// ============================================================================

/// <summary>
/// Parameters for generating product descriptions
/// </summary>
public class ProductDescriptionParameters
{
    [Required]
    [Description("The name of the product")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Description("The product part number or SKU")]
    public string PartNumber { get; set; } = string.Empty;

    [Required]
    [Description("List of product attributes, features, or specifications (e.g., ['Brand: DEWALT', 'Voltage: 20V', 'Capacity: 28 oz.'])")]
    public List<string> Attributes { get; set; } = new List<string>();
}

// ============================================================================
// Tool Implementation
// ProductDescriptionGeneratorTool with [OpalTool] attribute
// ============================================================================

/// <summary>
/// Product Description Generator Tool
/// Generates natural, AI-like product descriptions dynamically
/// </summary>
public class ProductDescriptionGeneratorTool
{
    [OpalTool(Name = "product-description-generator")]
    [Description("Generates natural, AI-like product descriptions (up to 500 characters) dynamically based on any product attributes")]
    public object GenerateDescription(ProductDescriptionParameters parameters)
    {
        // Validate parameters
        if (string.IsNullOrWhiteSpace(parameters.ProductName))
        {
            throw new ArgumentException("ProductName is required");
        }

        if (string.IsNullOrWhiteSpace(parameters.PartNumber))
        {
            throw new ArgumentException("PartNumber is required");
        }

        if (parameters.Attributes == null || !parameters.Attributes.Any())
        {
            throw new ArgumentException("Attributes array is required and must not be empty");
        }

        // Generate the description
        var description = GenerateProductDescription(
            parameters.ProductName,
            parameters.PartNumber,
            parameters.Attributes
        );

        // Return result with metadata
        return new
        {
            content = description,
            productName = parameters.ProductName,
            partNumber = parameters.PartNumber,
            attributeCount = parameters.Attributes.Count
        };
    }

    /// <summary>
    /// Generate natural, AI-like product description dynamically
    /// Works with any attributes - no hardcoding
    /// </summary>
    private static string GenerateProductDescription(
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
}
