namespace ProductDescriptionGenerator.Services;

/// <summary>
/// Service for generating natural, AI-like product descriptions
/// </summary>
public class DescriptionGenerationService : IDescriptionGenerationService
{
    private static readonly string[] PriorityKeys = { "capacity", "cartridge type", "weight", "dimensions", "material" };

    public string GenerateProductDescription(string productName, string partNumber, List<string> attributes, string type, string tone)
    {
        var (attrMap, attrList) = ParseAttributes(attributes);
        
        var description = BuildDescription(productName, partNumber, attrMap, attrList, type, tone);
        
        return description;
    }

    private static (Dictionary<string, string> attrMap, List<(string key, string value, string original)> attrList) 
        ParseAttributes(List<string> attributes)
    {
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

        return (attrMap, attrList);
    }

    private static string BuildDescription(
        string productName, 
        string partNumber, 
        Dictionary<string, string> attrMap, 
        List<(string key, string value, string original)> attrList,
        string type,
        string tone)
    {
        var description = $"The {productName} (Part# {partNumber}) ";

        description += GetOpeningStatement(attrMap, type, tone);
        description += GetFeatureHighlights(attrList);
        description += GetBrandStatement(attrMap);
        description += GetWarrantyStatement(attrMap);

        return description.Trim();
    }

    private static string GetOpeningStatement(Dictionary<string, string> attrMap, string type, string tone)
    {
        var hasPower = attrMap.ContainsKey("battery voltage (v)") ||
                       attrMap.ContainsKey("voltage") ||
                       attrMap.ContainsKey("power");
        var isCordless = attrMap.TryGetValue("cordless / corded", out var cordlessValue) &&
                         cordlessValue.ToLower() == "cordless";

        // Adjust opening based on type and tone
        if (type.ToLower() == "ecommerce" && tone.ToLower() == "professional")
        {
            if (hasPower && isCordless)
            {
                return "delivers powerful, reliable cordless performance for professional applications. ";
            }
            else if (hasPower)
            {
                return "offers consistent, professional-grade powered performance. ";
            }
            else
            {
                return "provides exceptional professional-grade quality and reliability. ";
            }
        }

        // Default behavior
        if (hasPower && isCordless)
        {
            return "delivers powerful cordless performance. ";
        }
        else if (hasPower)
        {
            return "offers reliable powered performance. ";
        }
        else
        {
            return "provides professional-grade quality. ";
        }
    }

    private static string GetFeatureHighlights(List<(string key, string value, string original)> attrList)
    {
        var meaningfulAttrs = attrList
            .Where(a => IsFeatureAttribute(a))
            .OrderBy(a => GetPriority(a.key))
            .Take(3)
            .ToList();

        if (!meaningfulAttrs.Any())
        {
            return string.Empty;
        }

        var features = string.Join(", ", meaningfulAttrs.Select(FormatFeature));
        return $"Features include {features}. ";
    }

    private static bool IsFeatureAttribute((string key, string value, string original) attr)
    {
        return !attr.key.Contains("cs_") &&
               attr.key != "brand" &&
               attr.key != "cordless / corded" &&
               attr.value.ToLower() != "yes" &&
               attr.value.ToLower() != "no" &&
               attr.value.Length > 2;
    }

    private static int GetPriority(string key)
    {
        return PriorityKeys.Any(k => key.Contains(k)) ? 0 : 1;
    }

    private static string FormatFeature((string key, string value, string original) attr)
    {
        // For better readability, include key for context if value doesn't already contain it
        if (attr.value.Length < 15 && !attr.value.ToLower().Contains(attr.key.Split(' ')[0]))
        {
            return $"{attr.key}: {attr.value}";
        }
        return attr.value;
    }

    private static string GetBrandStatement(Dictionary<string, string> attrMap)
    {
        if (attrMap.TryGetValue("brand", out var brand))
        {
            return $"Built with {brand} quality and reliability. ";
        }
        return string.Empty;
    }

    private static string GetWarrantyStatement(Dictionary<string, string> attrMap)
    {
        if (attrMap.TryGetValue("cs_manufacturer_warranty", out var warranty))
        {
            var warrantySimple = warranty.Replace(" limited warranty", "").Split('/')[0];
            return $"Backed by {warrantySimple} warranty. ";
        }
        return "Designed for demanding applications. ";
    }
}

