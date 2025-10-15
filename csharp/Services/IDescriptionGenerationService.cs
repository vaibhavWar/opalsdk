namespace ProductDescriptionGenerator.Services;

/// <summary>
/// Service interface for generating product descriptions
/// </summary>
public interface IDescriptionGenerationService
{
    /// <summary>
    /// Generate natural, AI-like product description dynamically
    /// </summary>
    /// <param name="productName">The name of the product</param>
    /// <param name="partNumber">The product part number</param>
    /// <param name="attributes">List of product attributes</param>
    /// <param name="type">The type or category of content</param>
    /// <param name="tone">The tone of the description</param>
    /// <returns>Generated product description</returns>
    string GenerateProductDescription(string productName, string partNumber, List<string> attributes, string type, string tone);
}

