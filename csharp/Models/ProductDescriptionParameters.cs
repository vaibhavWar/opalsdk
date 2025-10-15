using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductDescriptionGenerator.Models;

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

