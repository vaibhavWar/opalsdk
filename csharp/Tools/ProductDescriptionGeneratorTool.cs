using System.ComponentModel;
using Optimizely.Opal.Tools;
using ProductDescriptionGenerator.Models;
using ProductDescriptionGenerator.Services;

namespace ProductDescriptionGenerator.Tools;

/// <summary>
/// Product Description Generator Tool
/// Generates natural, AI-like product descriptions dynamically
/// </summary>
public class ProductDescriptionGeneratorTool
{
    private readonly IDescriptionGenerationService _descriptionService;

    public ProductDescriptionGeneratorTool(IDescriptionGenerationService descriptionService)
    {
        _descriptionService = descriptionService;
    }

    [OpalTool(Name = "product-description-generator")]
    [Description("Generates natural, AI-like product descriptions dynamically based on any product attributes")]
    public object GenerateDescription(ProductDescriptionParameters parameters)
    {
        // Validate parameters
        ValidateParameters(parameters);

        // Generate the description
        var description = _descriptionService.GenerateProductDescription(
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

    private static void ValidateParameters(ProductDescriptionParameters parameters)
    {
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
    }
}

